"""Offline UML diagram renderer for BankApp docs.

Renders class diagrams, package/component diagrams and sequence diagrams
to PNG using Pillow only (no network, no PlantUML/mermaid needed).
Mermaid (.mmd) sources are written separately alongside the PNGs.

All public coordinates are in final pixels; images are rendered 2x
supersampled and downscaled for crisp output.
"""
import math
import os

from PIL import Image, ImageDraw, ImageFont

S = 2  # supersampling factor

INK = (35, 41, 55)
LINE = (60, 70, 90)
WHITE = (255, 255, 255)
GRAY = (150, 150, 160)

PALETTE = {
    'class':     {'head': (221, 235, 247), 'border': (68, 114, 168), 'body': (249, 251, 253)},
    'abstract':  {'head': (221, 235, 247), 'border': (68, 114, 168), 'body': (249, 251, 253)},
    'interface': {'head': (226, 239, 218), 'border': (84, 130, 53),  'body': (248, 251, 247)},
    'enum':      {'head': (255, 242, 204), 'border': (191, 144, 0),  'body': (255, 252, 244)},
    'record':    {'head': (231, 230, 250), 'border': (103, 78, 167), 'body': (250, 249, 253)},
    'component': {'head': (234, 226, 243), 'border': (112, 76, 158), 'body': (247, 243, 250)},
    'package':   {'head': (208, 216, 221), 'border': (90, 105, 115), 'body': (240, 244, 246)},
    'actor':     {'head': (250, 224, 212), 'border': (192, 80, 42),  'body': (253, 246, 242)},
}

STEREOTYPE = {
    'interface': '«interface»',
    'enum': '«enumeration»',
    'record': '«record»',
    'static': '«utility»',
}

_FONT_FILES = {
    'title': ['segoeuib.ttf', 'arialbd.ttf'],
    'body': ['consola.ttf', 'cascadiacode.ttf'],
    'label': ['segoeuii.ttf', 'segoeuii_0.ttf', 'segoeui.ttf'],
    'small': ['segoeui.ttf', 'arial.ttf'],
}


def _font(kind, px):
    for f in _FONT_FILES[kind]:
        p = os.path.join('C:/Windows/Fonts', f)
        if os.path.exists(p):
            return ImageFont.truetype(p, int(px * S))
    return ImageFont.truetype('C:/Windows/Fonts/consola.ttf', int(px * S))


def text_w(font, s):
    return font.getlength(s)


def text_h(font):
    a, d = font.getmetrics()
    return a + d


def dashed_line(draw, p1, p2, fill, width=2, dash=7, gap=5):
    x1, y1 = p1
    x2, y2 = p2
    dx, dy = x2 - x1, y2 - y1
    dist = math.hypot(dx, dy)
    if dist == 0:
        return
    ux, uy = dx / dist, dy / dist
    t = 0.0
    while t < dist:
        t2 = min(t + dash, dist)
        draw.line([x1 + ux * t, y1 + uy * t, x1 + ux * t2, y1 + uy * t2], fill=fill, width=width)
        t = t2 + gap


def clip_to_rect(rect, towards):
    """Point on rect's border where segment rect-center -> towards exits."""
    x0, y0, x1, y1 = rect
    cx, cy = (x0 + x1) / 2, (y0 + y1) / 2
    tx, ty = towards
    dx, dy = tx - cx, ty - cy
    if dx == 0 and dy == 0:
        return cx, cy
    ts = []
    if dx != 0:
        ts.append(((x1 if dx > 0 else x0) - cx) / dx)
    if dy != 0:
        ts.append(((y1 if dy > 0 else y0) - cy) / dy)
    t = min(t for t in ts if t > 0)
    return cx + dx * t, cy + dy * t


def arrow_head(draw, tip, angle, kind, size=11):
    """Arrowhead with TIP at `tip`, pointing along `angle` (radians)."""
    x, y = tip
    a1 = angle + math.radians(158)
    a2 = angle - math.radians(158)
    p1 = (x + size * math.cos(a1), y + size * math.sin(a1))
    p2 = (x + size * math.cos(a2), y + size * math.sin(a2))
    if kind == 'triangle_hollow':
        draw.polygon([tip, p1, p2], fill=WHITE, outline=LINE)
        draw.line([tip, p1], fill=LINE, width=2)
        draw.line([tip, p2], fill=LINE, width=2)
        draw.line([p1, p2], fill=LINE, width=2)
    elif kind == 'triangle_filled':
        draw.polygon([tip, p1, p2], fill=LINE)
    elif kind == 'open':
        draw.line([p1, tip], fill=LINE, width=2)
        draw.line([p2, tip], fill=LINE, width=2)


def diamond(draw, tip, angle, filled, size=13):
    """Diamond attached at `tip` (on the owner border), extending along angle.

    Returns the far vertex (where the connecting line should start)."""
    x, y = tip
    back = (x + size * math.cos(angle), y + size * math.sin(angle))
    a1 = angle + math.radians(90)
    a2 = angle - math.radians(90)
    w = size * 0.5
    mx, my = (x + back[0]) / 2, (y + back[1]) / 2
    p1 = (mx + w * math.cos(a1), my + w * math.sin(a1))
    p2 = (mx + w * math.cos(a2), my + w * math.sin(a2))
    draw.polygon([tip, p1, back, p2], fill=(LINE if filled else WHITE), outline=LINE)
    return back


class ClassNode:
    """UML class/interface/enum/component node placed on a grid (col, row)."""

    def __init__(self, name, title=None, kind='class', attrs=(), ops=(), notes=(),
                 col=0, row=0, stereo=None):
        self.name = name
        self.title = title or name
        self.kind = kind
        self.attrs = list(attrs)
        self.ops = list(ops)
        self.notes = list(notes)
        self.col = col
        self.row = row
        self.stereo = stereo if stereo is not None else STEREOTYPE.get(kind)
        self.x = self.y = self.w = self.h = 0.0

    def measure(self, f_title, f_stereo, f_body):
        pad = 10
        self._head_h = max(24, text_h(f_title) + 8)
        if self.stereo:
            self._head_h += text_h(f_stereo) + 2
        self._lh = int(text_h(f_body) * 1.2)
        width = text_w(f_title, self.title)
        if self.stereo:
            width = max(width, text_w(f_stereo, self.stereo))
        self._comp_h = []
        for items in (self.attrs, self.ops, self.notes):
            if not items:
                self._comp_h.append(0)
                continue
            for it in items:
                width = max(width, text_w(f_body, it))
            self._comp_h.append(len(items) * self._lh + 10)
        self.w = width / S + 2 * pad
        h = self._head_h
        for ch in self._comp_h:
            if ch:
                h += ch
        self.h = h / S + 2

    def draw(self, dr, ox, oy, f_title, f_stereo, f_body):
        x0 = (ox + self.x) * S
        y0 = (oy + self.y) * S
        x1 = x0 + self.w * S
        y1 = y0 + self.h * S
        pal = PALETTE.get(self.kind, PALETTE['class'])
        head_h = self._head_h
        if self.kind == 'component' or self.kind == 'package':
            dr.rounded_rectangle([x0, y0, x1, y1], radius=7 * S,
                                 fill=pal['body'], outline=pal['border'], width=2)
            dr.rounded_rectangle([x0, y0, x1, y0 + head_h], radius=7 * S,
                                 fill=pal['head'], outline=pal['border'], width=2)
        else:
            dr.rectangle([x0, y0, x1, y1], fill=pal['body'], outline=pal['border'], width=2)
            dr.rectangle([x0, y0, x1, y0 + head_h], fill=pal['head'],
                         outline=pal['border'], width=2)
        cx = (x0 + x1) / 2
        if self.stereo:
            dr.text((cx, y0 + 3), self.stereo, font=f_stereo, fill=LINE, anchor='ma')
            ty = y0 + 5 + text_h(f_stereo)
        else:
            ty = y0 + 5
        dr.text((cx, ty), self.title, font=f_title, fill=INK, anchor='ma')
        if self.kind == 'abstract':
            tw = text_w(f_title, self.title)
            ly = ty + text_h(f_title) - 2
            dr.line([cx - tw / 2, ly, cx + tw / 2, ly], fill=INK, width=2)
        cy = y0 + head_h
        for idx, items in enumerate((self.attrs, self.ops, self.notes)):
            if not items:
                continue
            dr.line([x0, cy, x1, cy], fill=pal['border'], width=2)
            ty = cy + 6
            for it in items:
                dr.text((x0 + 9 * S, ty), it, font=f_body, fill=INK)
                ty += self._lh
            cy += self._comp_h[idx]

    @property
    def rect(self):
        return (self.x, self.y, self.x + self.w, self.y + self.h)

    @property
    def center(self):
        return (self.x + self.w / 2, self.y + self.h / 2)


class Edge:
    def __init__(self, src, dst, kind, label=None):
        self.src = src
        self.dst = dst
        self.kind = kind   # extends|implements|uses|assoc|composes|aggregates
        self.label = label


def _layout(nodes, hgap, vgap):
    cols, rows = {}, {}
    for n in nodes:
        cols[n.col] = max(cols.get(n.col, 0), n.w)
        rows[n.row] = max(rows.get(n.row, 0), n.h)
    xs, x = {}, 0.0
    for c in sorted(cols):
        xs[c] = x
        x += cols[c] + hgap
    ys, y = {}, 0.0
    for r in sorted(rows):
        ys[r] = y
        y += rows[r] + vgap
    for n in nodes:
        n.x = xs[n.col]
        n.y = ys[n.row]
    return (x - hgap, y - vgap)


def render_class_diagram(path, nodes, edges, title=None, hgap=54, vgap=58, margin=24):
    f_title = _font('title', 13)
    f_stereo = _font('small', 10.5)
    f_body = _font('body', 11.5)
    f_label = _font('label', 10.5)
    for n in nodes:
        n.measure(f_title, f_stereo, f_body)
    tw, th = _layout(nodes, hgap, vgap)
    title_h = 30 if title else 0
    W = margin * 2 + tw
    H = margin * 2 + title_h + th
    img = Image.new('RGB', (int(W * S), int(H * S)), WHITE)
    dr = ImageDraw.Draw(img)
    if title:
        dr.text((margin * S, (margin - 18) * S), title, font=_font('title', 14), fill=INK)
    oy = margin + title_h
    by_name = {n.name: n for n in nodes}
    for n in nodes:
        n.draw(dr, margin, oy, f_title, f_stereo, f_body)
    for e in edges:
        a, b = by_name[e.src], by_name[e.dst]
        ac, bc = a.center, b.center
        pa = clip_to_rect(a.rect, bc)
        pb = clip_to_rect(b.rect, ac)
        x1, y1 = (margin + pa[0]) * S, (oy + pa[1]) * S
        x2, y2 = (margin + pb[0]) * S, (oy + pb[1]) * S
        ang = math.atan2(y2 - y1, x2 - x1)
        solid = e.kind in ('extends', 'assoc', 'composes', 'aggregates')
        start = (x1, y1)
        if e.kind in ('composes', 'aggregates'):
            start = diamond(dr, (x1, y1), ang, e.kind == 'composes')
        if solid:
            dr.line([start[0], start[1], x2, y2], fill=LINE, width=2)
        else:
            dashed_line(dr, start, (x2, y2), LINE, width=2)
        if e.kind in ('extends', 'implements'):
            arrow_head(dr, (x2, y2), ang, 'triangle_hollow')
        elif e.kind in ('uses', 'assoc', 'composes', 'aggregates'):
            arrow_head(dr, (x2, y2), ang, 'open', size=10)
        if e.label:
            mx, my = (x1 + x2) / 2, (y1 + y2) / 2
            if abs(y2 - y1) >= abs(x2 - x1):
                mx += 8 * S
            else:
                my -= 7 * S
            tw_ = text_w(f_label, e.label)
            th_ = text_h(f_label)
            dr.rectangle([mx - tw_ / 2 - 3 * S, my - th_ - 2 * S,
                          mx + tw_ / 2 + 3 * S, my + 2 * S], fill=WHITE)
            dr.text((mx, my - th_ + 1), e.label, font=f_label, fill=LINE, anchor='ma')
    img = img.resize((int(W), int(H)), Image.LANCZOS)
    img.save(path)


# ---------------------------------------------------------------- sequences

class _Seq:
    def __init__(self, order, xs):
        self.order = order
        self.x = xs
        self.acts = {pid: [] for pid in order}
        self.tops = {pid: -1 for pid in order}

    def open_act(self, pid, y):
        self.acts[pid].append([y, y])
        self.tops[pid] += 1

    def extend_act(self, pid, y):
        stack = self.acts[pid]
        if stack:
            stack[-1][1] = y

    def close_act(self, pid, y):
        self.extend_act(pid, y)
        if self.acts[pid]:
            self.acts[pid].pop()
            self.tops[pid] -= 1


def _item_height(it):
    kind = it[0]
    if kind == 'msg':
        return 32
    if kind == 'self':
        return 46
    if kind == 'note':
        return len(it[2].split('\n')) * 16 + 14
    if kind == 'frame':
        h = 26
        for blabel, bitems in it[2]:
            h += 14 + sum(_item_height(s) for s in bitems)
        return h + 6
    return 0


def _draw_seq_items(items, dr, seq, y, f_label, f_small, width):
    for it in items:
        kind = it[0]
        if kind == 'msg':
            src, dst, label = it[1], it[2], it[3]
            mkind = it[4] if len(it) > 4 else 'call'
            x1 = seq.x[src] * S
            x2 = seq.x[dst] * S
            ym = y * S
            if mkind == 'call':
                seq.extend_act(src, y)
                seq.open_act(dst, y)
                dr.line([x1, ym, x2, ym], fill=LINE, width=2)
                arrow_head(dr, (x2, ym), 0 if x2 > x1 else math.pi, 'triangle_filled', size=9)
            else:
                seq.close_act(src, y)
                seq.extend_act(dst, y)
                dashed_line(dr, (x1, ym), (x2, ym), LINE, width=2)
                arrow_head(dr, (x2, ym), 0 if x2 > x1 else math.pi, 'open', size=10)
            cx = (x1 + x2) / 2
            tw_ = text_w(f_label, label)
            th_ = text_h(f_label)
            dr.rectangle([cx - tw_ / 2 - 3 * S, ym - th_ - 6 * S,
                          cx + tw_ / 2 + 3 * S, ym - 4 * S], fill=WHITE)
            dr.text((cx, ym - th_ - 5 * S), label, font=f_label, fill=INK, anchor='ma')
            y += 32
        elif kind == 'self':
            pid, label = it[1], it[2]
            x = seq.x[pid] * S
            ym = y * S
            seq.open_act(pid, y)
            flip = x > (width / 2) * S  # draw loop toward the diagram centre
            w = -36 * S if flip else 36 * S
            h = 24
            dr.line([x, ym, x + w, ym], fill=LINE, width=2)
            dr.line([x + w, ym, x + w, ym + h], fill=LINE, width=2)
            dr.line([x + w, ym + h, x, ym + h], fill=LINE, width=2)
            arrow_head(dr, (x, ym + h), 0 if flip else math.pi, 'triangle_filled', size=9)
            tw_ = text_w(f_label, label)
            th_ = text_h(f_label)
            lx = x + w - 3 * S if flip else x + w + 3 * S
            dr.rectangle([lx - tw_ / 2 - 3 * S, ym - th_ - 8 * S,
                          lx + tw_ / 2 + 3 * S, ym - 6 * S], fill=WHITE)
            dr.text((lx, ym - th_ - 7 * S), label, font=f_label, fill=INK, anchor='ma')
            y += 24 + 22
            seq.close_act(pid, y)
        elif kind == 'note':
            pid, text_ = it[1], it[2]
            lines = text_.split('\n')
            x = seq.x[pid] * S
            h = len(lines) * 16 + 10
            ym = (y + 2) * S
            wmax = max(text_w(f_small, ln) for ln in lines)
            dr.rectangle([x - wmax / 2 - 9 * S, ym,
                          x + wmax / 2 + 9 * S, ym + h * S],
                         fill=(255, 248, 214), outline=(191, 144, 0), width=2)
            ty = ym + 3 * S
            for ln in lines:
                dr.text((x, ty), ln, font=f_small, fill=INK, anchor='ma')
                ty += 16 * S
            y += h + 6
        elif kind == 'frame':
            fk, branches = it[1], it[2]
            ftop = y + 2
            inner = ftop + 24
            fx0, fx1 = 14 * S, (width - 14) * S
            for bi, (blabel, bitems) in enumerate(branches):
                if bi > 0:
                    dashed_line(dr, (fx0, (inner - 2) * S), (fx1, (inner - 2) * S), LINE,
                                width=1, dash=5, gap=4)
                    inner += 14
                dr.text((fx0 + 10 * S, (inner - 13) * S), blabel, font=f_small, fill=LINE,
                        anchor='la')
                inner = _draw_seq_items(bitems, dr, seq, inner + 4, f_label, f_small, width)
            fbot = inner + 8
            dr.rectangle([fx0, ftop * S, fx1, fbot * S], outline=LINE, width=2)
            tabw = text_w(f_small, fk) + 12 * S
            dr.rectangle([fx0, ftop * S, fx0 + tabw, (ftop + 17) * S],
                         fill=(235, 238, 244), outline=LINE, width=2)
            dr.text((fx0 + 5 * S, (ftop + 2) * S), fk, font=f_small, fill=INK, anchor='la')
            y = fbot
    return y


def render_sequence(path, participants, items, title=None, margin=26):
    """participants: list of (id, name, stereo). Items: tuples, see module docstring."""
    f_label = _font('label', 11)
    f_small = _font('small', 10.5)
    f_name = _font('title', 12.5)
    f_stereo = _font('small', 10)
    order = [p[0] for p in participants]
    pw = {}
    for pid, name, stereo in participants:
        w = text_w(f_name, name) / S + 28
        if stereo:
            w = max(w, text_w(f_stereo, stereo) / S + 28)
        pw[pid] = w
    gap = 108
    xs = {}
    x = margin
    for pid in order:
        xs[pid] = x + pw[pid] / 2
        x += pw[pid] + gap
    seq = _Seq(order, xs)
    title_h = 30 if title else 0
    head_h = 34
    seq_h = sum(_item_height(it) for it in items)
    H = margin + title_h + head_h + seq_h + 36
    W = x - gap + margin
    img = Image.new('RGB', (int(W * S), int(H * S)), WHITE)
    dr = ImageDraw.Draw(img)
    if title:
        dr.text((margin * S, (margin - 18) * S), title, font=_font('title', 14), fill=INK)
    top = margin + title_h
    for pid, name, stereo in participants:
        x0 = (xs[pid] - pw[pid] / 2) * S
        x1 = (xs[pid] + pw[pid] / 2) * S
        y1 = (top + head_h) * S
        pal = PALETTE['actor' if stereo == 'actor' else 'class']
        dr.rounded_rectangle([x0, top * S, x1, y1], radius=6 * S,
                             fill=pal['head'], outline=LINE, width=2)
        if stereo and stereo != 'actor':
            dr.text((xs[pid] * S, (top + 2) * S), stereo, font=f_stereo, fill=LINE, anchor='ma')
            dr.text((xs[pid] * S, (top + 15) * S), name, font=f_name, fill=INK, anchor='ma')
        else:
            dr.text((xs[pid] * S, (top + 8) * S), name, font=f_name, fill=INK, anchor='ma')
    ytop = top + head_h
    ybot = (H - 22) * S
    for pid in order:
        dashed_line(dr, (xs[pid] * S, ytop * S), (xs[pid] * S, ybot), GRAY, width=1, dash=5, gap=5)
    ystart = ytop + 14
    seq.open_act(order[0], ystart)
    yend = _draw_seq_items(items, dr, seq, ystart, f_label, f_small, W)
    seq.close_act(order[0], yend)
    for pid in order:
        for (ya, yb) in seq.acts[pid]:
            xa = xs[pid] * S
            dr.rectangle([xa - 5 * S, ya * S - 2 * S, xa + 5 * S, yb * S + 2 * S],
                         fill=(232, 236, 244), outline=LINE, width=1)
    img = img.resize((int(W), int(H)), Image.LANCZOS)
    img.save(path)
