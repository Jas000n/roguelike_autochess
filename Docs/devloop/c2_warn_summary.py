#!/usr/bin/env python3
import glob
import os
import re
from statistics import mean

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(__file__)))
builds = os.path.join(ROOT, "Builds")
paths = sorted(glob.glob(os.path.join(builds, "build_devloop_cycle_c2_warn_sample_*.log")))

rows = []
for p in paths:
    txt = open(p, encoding="utf-8", errors="ignore").read()
    m = re.search(r"\[DEV\]\[SPIKE_SCENARIO\] pass=(\d+) fail=(\d+) warn=(\d+)", txt)
    if not m:
        continue
    sample = int(re.search(r"sample_(\d+)\.log$", p).group(1))
    warn = int(m.group(3))
    passed = "[DEV][BATCH] PASSED" in txt
    rows.append((sample, warn, passed))

rows.sort(key=lambda x: x[0])
if not rows:
    print("no samples found")
    raise SystemExit(0)

def summarize(name, data):
    warn_runs = sum(1 for _, w, _ in data if w > 0)
    warn_total = sum(w for _, w, _ in data)
    pass_rate = mean(1.0 if p else 0.0 for _, _, p in data)
    print(f"[{name}] samples={len(data)} warn_runs={warn_runs} warn_total={warn_total} pass_rate={pass_rate:.2f}")

summarize("all", rows)
recent = rows[-10:]
summarize("recent10", recent)
print("recent10 detail:", ", ".join(f"s{n}:w{w}" for n, w, _ in recent))

# Draft soft-gate recommendation
recent_warn_runs = sum(1 for _, w, _ in recent if w > 0)
if recent_warn_runs >= 5:
    print("recommendation: TRIGGER soft-gate (recent10 warn_runs >= 5)")
else:
    print("recommendation: keep warn-only (recent10 warn_runs < 5)")
