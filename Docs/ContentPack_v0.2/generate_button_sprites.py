#!/usr/bin/env python3
import base64
import json
import os
import sys
from pathlib import Path
from urllib import request

API = os.environ.get("DRAWTHINGS_URL", "http://127.0.0.1:7860").rstrip("/")
MODEL = os.environ.get("DRAWTHINGS_MODEL", "qwen_image_2512_q6p.ckpt")
REQ_TIMEOUT = int(os.environ.get("DRAWTHINGS_TIMEOUT", "1200"))
BASE_DIR = Path(__file__).resolve().parent
OUT = BASE_DIR / "generated_ui"
OUT.mkdir(parents=True, exist_ok=True)

NEG = (
    "full interface, whole ui screen, dashboard, panel layout, menu screen, HUD, multiple panels,"
    " text, letters, logo, watermark, icon, character, scenery, complex background, photorealistic,"
    " blurry, lowres, noise, border artifacts, perspective view"
)

PROMPTS = {
    "ui_button": (
        "single standalone game ui button sprite, exactly one object in frame, rounded rectangle, centered,"
        " orthographic front-facing view, no perspective tilt, cyan-to-azure horizontal gradient fill,"
        " smooth beveled edge, subtle glossy lacquer highlight near top edge, metallic thin border,"
        " clean anti-aliased silhouette, generous transparent margin around the button, no shadow spill outside,"
        " no background scene, neutral dark plain backdrop only, no icon, no text, no glyph, no pattern,"
        " professional strategy game hud asset, export-ready sprite sheet element"
    ),
    "ui_button_hover": (
        "single standalone game ui button sprite, exactly one object in frame, rounded rectangle, centered,"
        " orthographic front-facing view, hover-state variant of base button, brighter cyan-blue core,"
        " stronger edge bloom and soft outer aura, intensified rim light, crisp bevel separation,"
        " face area intentionally empty for runtime text overlay, clean anti-aliased silhouette,"
        " no perspective distortion, no icons, no letters, no logos, no decorative scene elements,"
        " plain dark neutral backdrop only, premium polished interactive hover feedback style"
    ),
    "ui_button_pressed": (
        "single standalone game ui button sprite, exactly one object in frame, rounded rectangle, centered,"
        " orthographic front-facing view, pressed-state variant, deep navy-cyan tone, visible inset cavity,"
        " compressed highlight band, reduced outer glow, stronger inner shadow near top and side edges,"
        " tactile depressed interaction feel, clean anti-aliased contour, empty center label area,"
        " no text, no icon, no symbols, no scene background, plain dark neutral backdrop only,"
        " polished strategy game control element"
    ),
    "ui_button_warn": (
        "single standalone game ui button sprite, exactly one object in frame, rounded rectangle, centered,"
        " orthographic front-facing view, warning-state variant, amber-to-orange gradient, warm gold rim,"
        " subtle emissive edge, glossy protective coating highlight, high contrast but still readable,"
        " clear empty label area for in-game text overlay, clean anti-aliased border, no perspective,"
        " no icon, no text, no logo, no emblem, no background scene, plain dark neutral backdrop only,"
        " premium alert action button for fantasy strategy interface"
    ),
}


def txt2img(prompt: str, out_path: Path):
    payload = {
        "prompt": prompt + ", no text, no logo, no symbols, no icons, game ui sprite",
        "negative_prompt": NEG,
        "steps": 16,
        "sampler_name": "Euler a",
        "cfg_scale": 7.0,
        "width": 768,
        "height": 320,
        "batch_size": 1,
        "seed": -1,
        "model": MODEL,
    }
    req = request.Request(
        API + "/sdapi/v1/txt2img",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    with request.urlopen(req, timeout=REQ_TIMEOUT) as resp:
        data = json.loads(resp.read().decode("utf-8"))
    out_path.write_bytes(base64.b64decode(data["images"][0]))


def ensure_server():
    paths = ("/sdapi/v1/sd-models", "/sdapi/v1/options", "/")
    last_err = None
    for path in paths:
        try:
            req = request.Request(API + path, method="GET")
            with request.urlopen(req, timeout=10) as resp:
                if resp.status == 200:
                    return
        except Exception as e:
            last_err = e
    print(f"[ERROR] Draw Things WebAPI 不可用: {API}")
    print("请在 Draw Things 里开启 Web Server/API，或设置正确端口后重试。")
    print(f"原始错误: {last_err}")
    sys.exit(2)


if __name__ == "__main__":
    ensure_server()
    print(f"[INFO] Draw Things API: {API}")
    print(f"[INFO] Target model: {MODEL}")
    print(f"[INFO] Request timeout: {REQ_TIMEOUT}s")
    only = os.environ.get("BUTTON_ONLY", "").strip()
    tasks = PROMPTS.items() if not only else [(only, PROMPTS[only])]
    for name, prompt in tasks:
        out = OUT / f"{name}.png"
        txt2img(prompt, out)
        print("generated", out)
