"""Programmatic QA for the rendered diagrams.

Re-runs the layout and checks:
  1. no two node boxes overlap
  2. no edge segment passes through an unrelated node box
  3. edge labels do not overlap node boxes
  4. sequence notes/labels stay inside the canvas
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import diagrams
from uml_render import _layout, _font, text_w, text_h, S, clip_to_rect

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.abspath(os.path.join(HERE, '..', 'diagrams'))

f_title = _font('title', 13)
f_stereo = _font('small', 10.5)
f_body = _font('body', 11.5)
f_label = _font('label', 10.5)


def rects_overlap(a, b, tol=1.0):
    return not (a[2] <= b[0] + tol or b[2] <= a[0] + tol or
                a[3] <= b[1] + tol or b[3] <= a[1] + tol)


def seg_intersects_rect(p1, p2, rect, pad=2.0):
    """Sample-based segment vs rect test (rect shrunk by pad to ignore touching)."""
    x0, y0, x1, y1 = rect[0] + pad, rect[1] + pad, rect[2] - pad, rect[3] - pad
    steps = 120
    for i in range(steps + 1):
        t = i / steps
        x = p1[0] + (p2[0] - p1[0]) * t
        y = p1[1] + (p2[1] - p1[1]) * t
        if x0 < x < x1 and y0 < y < y1:
            return True
    return False


problems = []

for d in diagrams.DIAGRAMS:
    name = d['name']
    if d['kind'] != 'class':
        continue
    nodes, edges = d['data']
    for n in nodes:
        n.measure(f_title, f_stereo, f_body)
    _layout(nodes, 54, 58)
    by = {n.name: n for n in nodes}
    # 1. node overlap
    for i in range(len(nodes)):
        for j in range(i + 1, len(nodes)):
            if rects_overlap(nodes[i].rect, nodes[j].rect):
                problems.append(f'{name}: node overlap {nodes[i].name} / {nodes[j].name}')
    # 2. edge crossing unrelated boxes
    for e in edges:
        a, b = by[e.src], by[e.dst]
        pa = clip_to_rect(a.rect, b.center)
        pb = clip_to_rect(b.rect, a.center)
        for n in nodes:
            if n.name in (e.src, e.dst):
                continue
            if seg_intersects_rect(pa, pb, n.rect):
                problems.append(f'{name}: edge {e.src}->{e.dst} crosses {n.name}')
        # 3. label vs boxes
        if e.label:
            mx, my = (pa[0] + pb[0]) / 2, (pa[1] + pb[1]) / 2
            if abs(pb[1] - pa[1]) >= abs(pb[0] - pa[0]):
                mx += 8
            else:
                my -= 7
            tw_ = text_w(f_label, e.label) / S
            th_ = text_h(f_label) / S
            lab = (mx - tw_ / 2 - 3, my - th_ - 2, mx + tw_ / 2 + 3, my + 2)
            for n in nodes:
                if n.name in (e.src, e.dst):
                    continue
                if rects_overlap(lab, n.rect):
                    problems.append(f'{name}: label of {e.src}->{e.dst} overlaps {n.name}')

# --- sequence diagrams: canvas bounds check -------------------------------
for d in diagrams.DIAGRAMS:
    if d['kind'] != 'seq':
        continue
    name = d['name']
    parts = d['data']['participants']
    items = d['data']['items']
    f_name = _font('title', 12.5)
    f_label = _font('label', 11)
    pw = {}
    for pid, pname, stereo in parts:
        w = text_w(f_name, pname) / S + 28
        if stereo:
            w = max(w, text_w(_font('small', 10), stereo) / S + 28)
        pw[pid] = w
    gap = 108
    xs = {}
    x = 26
    for pid, _, _ in parts:
        xs[pid] = x + pw[pid] / 2
        x += pw[pid] + gap
    W = x - gap + 26

    def walk(its):
        for it in its:
            if it[0] == 'msg':
                src, dst, label = it[1], it[2], it[3]
                lo, hi = sorted([xs[src], xs[dst]])
                tw_ = text_w(f_label, label) / S
                cx = (lo + hi) / 2
                if cx - tw_ / 2 < 4 or cx + tw_ / 2 > W - 4:
                    problems.append(f'{name}: msg label out of canvas: "{label[:40]}"')
            elif it[0] == 'self':
                pid, label = it[1], it[2]
                tw_ = text_w(f_label, label) / S
                flip = xs[pid] > W / 2
                cx = xs[pid] - 36 - 2 if flip else xs[pid] + 36 + 2
                if cx + tw_ / 2 > W - 4 or cx - tw_ / 2 < 4:
                    problems.append(f'{name}: self label out of canvas: "{label[:40]}"')
            elif it[0] == 'note':
                pid, text_ = it[1], it[2]
                wmax = max(text_w(_font('small', 10.5), ln) for ln in text_.split('\n')) / S
                if xs[pid] - wmax / 2 - 9 < 4 or xs[pid] + wmax / 2 + 9 > W - 4:
                    problems.append(f'{name}: note out of canvas: "{text_[:40]}"')
            elif it[0] == 'frame':
                for _, bitems in it[2]:
                    walk(bitems)
    walk(items)

if problems:
    print(f'{len(problems)} PROBLEM(S):')
    for p in problems:
        print(' -', p)
    sys.exit(1)
print('layout QA: all checks passed')
