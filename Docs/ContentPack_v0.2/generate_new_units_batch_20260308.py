#!/usr/bin/env python3
import base64
import json
import os
from pathlib import Path
from urllib import request

API = os.environ.get("DRAWTHINGS_URL", "http://127.0.0.1:7860").rstrip("/")
MODEL = os.environ.get("DRAWTHINGS_MODEL", "qwen_image_2512_q6p.ckpt")
TIMEOUT = int(os.environ.get("DRAWTHINGS_TIMEOUT", "10800"))
ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "Assets/Resources/Art/Units"
OUT.mkdir(parents=True, exist_ok=True)

NEG = (
    "text, letters, logo, watermark, signature, blurry, lowres, noisy, jpeg artifacts, "
    "full game screenshot, full ui panel, interface layout, multiple subjects, collage, "
    "deformed anatomy, extra limbs, photorealistic, 3d render"
)

STYLE = (
    "single standalone strategy-game unit portrait asset, same art direction as existing DragonChessLegends units, "
    "neo-oriental mech fantasy with chinese chess flavor, clean silhouette readable at thumbnail size, "
    "centered composition, medium-close framing, dramatic rim light, high contrast value separation, "
    "dark navy background with subtle particles, no frame no border no ui no text"
)

UNITS = {
    "horse_wind": "swift wind cavalry horse with flowing cyan streamers, light armor, high-mobility assault posture, dynamic motion trails",
    "cannon_frost": "frost crystal artillery cannon with icy rail barrel and cold-core chamber, precise long-range threat silhouette",
    "guard_mist": "mist assassin guard with twin fog-edged daggers and phantom afterimages, high-speed execution stance",
    "guard_holy": "holy guardian lancer with radiant shield crest and sanctified steel armor, defensive protector silhouette",
    "soldier_zeal": "zeal infantry soldier with oath banner and compact blade, disciplined frontline fighter with holy undertone",
}


def ping_api():
    for p in ("/sdapi/v1/options", "/"):
        try:
            req = request.Request(API + p, method="GET")
            with request.urlopen(req, timeout=8) as resp:
                if resp.status == 200:
                    return True
        except Exception:
            continue
    return False


def txt2img(prompt: str, out_path: Path):
    payload = {
        "prompt": f"{prompt}, {STYLE}",
        "negative_prompt": NEG,
        "steps": 14,
        "sampler_name": "Euler a",
        "cfg_scale": 6.8,
        "width": 1024,
        "height": 1024,
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
    with request.urlopen(req, timeout=TIMEOUT) as resp:
        data = json.loads(resp.read().decode("utf-8"))
    out_path.write_bytes(base64.b64decode(data["images"][0]))


def main():
    if not ping_api():
        raise SystemExit(f"Draw Things API unavailable: {API}")
    print(f"Using API={API}")
    print(f"Using model={MODEL}")

    for key, desc in UNITS.items():
        out = OUT / f"unit_{key}.png"
        prompt = (
            f"single unit portrait of {desc}, strategy autobattler identity clear, "
            "must not contain any full scene, full interface, card sheet, or text"
        )
        txt2img(prompt, out)
        print(f"generated {out}", flush=True)


if __name__ == "__main__":
    main()
