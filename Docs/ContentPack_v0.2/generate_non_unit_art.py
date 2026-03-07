#!/usr/bin/env python3
import base64
import json
import os
import sys
from pathlib import Path
from urllib import request

API = os.environ.get("DRAWTHINGS_URL", "http://127.0.0.1:7860").rstrip("/")
MODEL = os.environ.get("DRAWTHINGS_MODEL", "qwen_image_2512_q6p.ckpt")
REQ_TIMEOUT = int(os.environ.get("DRAWTHINGS_TIMEOUT", "1800"))
BASE_DIR = Path(__file__).resolve().parent
OUT = BASE_DIR / "generated_non_units"
OUT.mkdir(parents=True, exist_ok=True)

NEG = (
    "character portrait, human, monster, full interface screenshot, dashboard, hud layout,"
    " text, letters, logo, watermark, signature, blurry, lowres, noisy, deformed,"
    " photorealistic, perspective scene, cluttered background, multiple objects"
)

STYLE = (
    "cohesive strategy game ui art style, neo-oriental fantasy with refined metallic trims,"
    " navy-cyan palette with controlled amber highlights, clean silhouette, production-ready"
)

ASSETS = [
    {
        "name": "ui_panel",
        "w": 1024,
        "h": 512,
        "prompt": (
            "single empty ui panel background texture, dark navy base, elegant bronze frame,"
            " subtle inner glow, broad clean center area for text and controls, no symbols,"
            " no labels, front-facing flat 2d"
        ),
        "dst": "Assets/Resources/Art/UI/ui_panel.png",
    },
    {
        "name": "ui_panel_dark",
        "w": 1024,
        "h": 512,
        "prompt": (
            "single empty dark ui panel texture, deeper navy-black tone, restrained bevel,"
            " calm contrast, thin metallic border accents, clean center space, front-facing flat 2d"
        ),
        "dst": "Assets/Resources/Art/UI/ui_panel_dark.png",
    },
    {
        "name": "ui_card",
        "w": 640,
        "h": 896,
        "prompt": (
            "single ui card frame texture, vertical card, polished metallic edge, dark readable center,"
            " minimal ornament, no icon, no text, front-facing flat 2d, card game quality"
        ),
        "dst": "Assets/Resources/Art/UI/ui_card.png",
    },
    {
        "name": "ui_button",
        "w": 768,
        "h": 320,
        "prompt": (
            "single standalone game ui button sprite, rounded rectangle, centered, orthographic front view,"
            " cyan-to-azure gradient, smooth bevel, subtle glossy highlight, empty face area for runtime text,"
            " no icon, no symbol"
        ),
        "dst": "Assets/Resources/Art/UI/ui_button.png",
    },
    {
        "name": "ui_button_hover",
        "w": 768,
        "h": 320,
        "prompt": (
            "single standalone game ui button sprite hover state, rounded rectangle, centered,"
            " brighter cyan core, stronger rim light and soft outer glow, clean bevel,"
            " empty face area, no icon, no text"
        ),
        "dst": "Assets/Resources/Art/UI/ui_button_hover.png",
    },
    {
        "name": "ui_button_pressed",
        "w": 768,
        "h": 320,
        "prompt": (
            "single standalone game ui button sprite pressed state, rounded rectangle, centered,"
            " deep navy-cyan tone, visible inset depression, reduced glow, stronger inner shadow,"
            " empty face area, no icon, no text"
        ),
        "dst": "Assets/Resources/Art/UI/ui_button_pressed.png",
    },
    {
        "name": "ui_button_warn",
        "w": 768,
        "h": 320,
        "prompt": (
            "single standalone game ui warning button sprite, rounded rectangle, centered,"
            " amber-to-orange gradient with gold rim, glossy highlight, strong readability,"
            " empty face area, no icon, no text"
        ),
        "dst": "Assets/Resources/Art/UI/ui_button_warn.png",
    },
    {
        "name": "board_bg",
        "w": 1280,
        "h": 768,
        "prompt": (
            "single chess battle background texture, moody dark arena floor, subtle geometric patterns,"
            " calm gradient, no characters, no objects, no text, seamless-looking broad composition"
        ),
        "dst": "Assets/Resources/Art/board_bg.png",
    },
    {
        "name": "tile_a",
        "w": 384,
        "h": 384,
        "prompt": (
            "single board tile texture variant A, square top-down tile, dark blue stone with clean edge,"
            " subtle worn details, no emblem, no text"
        ),
        "dst": "Assets/Resources/Art/tile_a.png",
    },
    {
        "name": "tile_b",
        "w": 384,
        "h": 384,
        "prompt": (
            "single board tile texture variant B, square top-down tile, slightly lighter navy stone,"
            " complementary style to variant A, subtle detail, no emblem, no text"
        ),
        "dst": "Assets/Resources/Art/tile_b.png",
    },
    {
        "name": "icon_dragon",
        "w": 512,
        "h": 512,
        "prompt": "single ui icon, stylized dragon head emblem, high contrast, centered, no text",
        "dst": "Assets/Resources/Art/icon_dragon.png",
    },
    {
        "name": "icon_horse",
        "w": 512,
        "h": 512,
        "prompt": "single ui icon, stylized horse head emblem, high contrast, centered, no text",
        "dst": "Assets/Resources/Art/icon_horse.png",
    },
    {
        "name": "icon_sword",
        "w": 512,
        "h": 512,
        "prompt": "single ui icon, stylized sword emblem, high contrast, centered, no text",
        "dst": "Assets/Resources/Art/icon_sword.png",
    },
    {
        "name": "icon_bomb",
        "w": 512,
        "h": 512,
        "prompt": "single ui icon, stylized bomb/artillery shell emblem, high contrast, centered, no text",
        "dst": "Assets/Resources/Art/icon_bomb.png",
    },
    {
        "name": "icon_shield",
        "w": 512,
        "h": 512,
        "prompt": "single ui icon, stylized shield emblem, high contrast, centered, no text",
        "dst": "Assets/Resources/Art/icon_shield.png",
    },
]

HEX_IDS = [
    "rich",
    "interest_up",
    "fast_train",
    "thrifty_refresh",
    "scrap_rebate",
    "team_atk",
    "vanguard_wall",
    "rider_charge",
    "cannon_master",
    "artillery_range",
    "board_plus",
    "healing",
    "overclocked_core",
    "frontline_oath",
    "precision_barrage",
]

HEX_PROMPTS = {
    "rich": "single hex augment icon, raining coins economy boost",
    "interest_up": "single hex augment icon, finance growth and interest upgrade",
    "fast_train": "single hex augment icon, accelerated training and level up",
    "thrifty_refresh": "single hex augment icon, reroll discount and thrift economy",
    "scrap_rebate": "single hex augment icon, salvage rebate and recycling value",
    "team_atk": "single hex augment icon, team-wide attack amplification aura",
    "vanguard_wall": "single hex augment icon, frontline steel wall defense",
    "rider_charge": "single hex augment icon, cavalry charge burst momentum",
    "cannon_master": "single hex augment icon, artillery mastery precision firepower",
    "artillery_range": "single hex augment icon, extended targeting range",
    "board_plus": "single hex augment icon, extra deployment slot on board",
    "healing": "single hex augment icon, battlefield sustain and repair pulse",
    "overclocked_core": "single hex augment icon, overclock core speed and power",
    "frontline_oath": "single hex augment icon, defensive oath of frontline units",
    "precision_barrage": "single hex augment icon, precision barrage strike pattern",
}

for hid in HEX_IDS:
    ASSETS.append(
        {
            "name": f"hex_{hid}",
            "w": 512,
            "h": 512,
            "prompt": HEX_PROMPTS[hid] + ", centered, icon-ready, no text",
            "dst": f"Assets/Resources/Art/Hexes/hex_{hid}.png",
        }
    )


def ensure_server():
    checks = ("/sdapi/v1/options", "/")
    last_err = None
    for path in checks:
        try:
            req = request.Request(API + path, method="GET")
            with request.urlopen(req, timeout=10) as resp:
                if resp.status == 200:
                    return
        except Exception as e:
            last_err = e
    print(f"[ERROR] Draw Things WebAPI unavailable: {API}")
    print(f"[ERROR] last: {last_err}")
    sys.exit(2)


def generate_one(item):
    full_prompt = f"{item['prompt']}, {STYLE}"
    payload = {
        "prompt": full_prompt,
        "negative_prompt": NEG,
        "steps": 8,
        "sampler_name": "Euler a",
        "cfg_scale": 7.0,
        "width": item["w"],
        "height": item["h"],
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

    image = base64.b64decode(data["images"][0])
    out_path = OUT / f"{item['name']}.png"
    out_path.write_bytes(image)

    dst = Path(item["dst"])
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_bytes(image)
    print(f"[OK] {item['name']} -> {out_path} -> {dst}")


def main():
    ensure_server()
    only = os.environ.get("ASSET_ONLY", "").strip()
    tasks = ASSETS
    if only:
        tasks = [a for a in ASSETS if a["name"] == only]
        if not tasks:
            print(f"[ERROR] unknown ASSET_ONLY={only}")
            sys.exit(2)

    print(f"[INFO] API={API}")
    print(f"[INFO] MODEL={MODEL}")
    print(f"[INFO] TIMEOUT={REQ_TIMEOUT}")
    print(f"[INFO] task_count={len(tasks)}")

    for i, item in enumerate(tasks, start=1):
        print(f"[{i}/{len(tasks)}] generating {item['name']} ...")
        generate_one(item)


if __name__ == "__main__":
    main()
