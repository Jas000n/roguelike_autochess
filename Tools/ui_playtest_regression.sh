#!/usr/bin/env bash
set -euo pipefail

# UI full-flow regression runner for DragonChessLegends macOS build.
# It performs deterministic interactions and captures checkpoints for manual review.

APP_PATH="${1:-/Users/jason/.openclaw/workspace/DragonChessLegends/Builds/Mac/DragonChessLegends.app}"
ART_BASE="${2:-/Users/jason/.openclaw/workspace/DragonChessLegends/Builds/playtest_ui_regression}"

if ! command -v cliclick >/dev/null 2>&1; then
  echo "cliclick not found. Install with: brew install cliclick" >&2
  exit 2
fi

if [[ ! -d "$APP_PATH" ]]; then
  echo "App not found: $APP_PATH" >&2
  exit 2
fi

TS="$(date +%Y%m%d_%H%M%S)"
ART_DIR="$ART_BASE/run_$TS"
mkdir -p "$ART_DIR"
LOG="$ART_DIR/playtest.log"

shot() {
  local name="$1"
  local out="$ART_DIR/${name}_$(date +%Y%m%d_%H%M%S).png"
  if command -v screencapture >/dev/null 2>&1; then
    screencapture -x "$out"
    echo "$(date '+%H:%M:%S') | screenshot | $out" | tee -a "$LOG"
  else
    echo "$(date '+%H:%M:%S') | skip screenshot | (screencapture not found)" | tee -a "$LOG"
  fi
}

step() {
  echo "$(date '+%H:%M:%S') | step | $1" | tee -a "$LOG"
}

focus_app() {
  osascript -e 'tell application "DragonChessLegends" to activate' >/dev/null 2>&1 || true
  sleep 0.25
}

keypress() {
  local keycode="$1"
  osascript -e "tell application \"System Events\" to key code ${keycode}" >/dev/null 2>&1 || true
}

# Coordinates tuned for default 1366x768 desktop run.
click() {
  local x="$1"
  local y="$2"
  /opt/homebrew/bin/cliclick c:${x},${y}
}

drag() {
  local x1="$1"
  local y1="$2"
  local x2="$3"
  local y2="$4"
  /opt/homebrew/bin/cliclick dd:${x1},${y1} du:${x2},${y2}
}

pkill -f "DragonChessLegends.app/Contents/MacOS/DragonChessLegends" >/dev/null 2>&1 || true

step "launch app"
open -na "$APP_PATH"
sleep 4.0
focus_app
shot "01_stage_initial"

step "enter prepare via F9"
keypress 120
sleep 1.0
focus_app
shot "02_prepare_initial"

step "run in-game ui smoke test (F7)"
keypress 98
sleep 0.8
shot "02b_ui_smoke_result"

step "toggle route fold/unfold"
click 1286 418
sleep 0.25
click 1286 418
sleep 0.25
shot "03_prepare_route_toggle"

step "shop ops: refresh, exp, lock"
click 696 650
sleep 0.25
click 825 650
sleep 0.25
click 955 650
sleep 0.25
shot "04_prepare_shop_ops"

step "buy from all shop slots if possible"
click 84 653
sleep 0.2
click 222 653
sleep 0.2
click 356 653
sleep 0.2
click 495 653
sleep 0.2
click 628 653
sleep 0.2
shot "05_prepare_after_buys"

step "drag bench to board"
drag 66 715 492 488
sleep 0.25
shot "06_prepare_drag_to_board"

step "drag bench to occupied board cell (swap test)"
drag 154 715 492 488
sleep 0.25
shot "07_prepare_swap_test"

step "drag board unit out of board (sell test)"
drag 516 458 1338 118
sleep 0.25
shot "08_prepare_sell_drag"

step "start battle (space)"
keypress 49
sleep 1.3
focus_app
shot "09_battle_start"

step "battle mid snapshot"
sleep 2.2
shot "10_battle_mid"

step "battle end snapshot"
sleep 5.0
shot "11_post_battle"

step "try pick reward if reward state visible"
click 112 366
sleep 0.35
shot "12_after_reward_pick_attempt"

step "try pick hex if hex state visible"
click 112 400
sleep 0.35
shot "13_after_hex_pick_attempt"

step "prepare final snapshot"
shot "14_prepare_final"

pkill -f "DragonChessLegends.app/Contents/MacOS/DragonChessLegends" >/dev/null 2>&1 || true
step "done: $ART_DIR"
echo "$ART_DIR"
