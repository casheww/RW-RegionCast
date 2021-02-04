import hashlib
from io import BytesIO
import json
import requests
from zipfile import ZipFile


if __name__ == "__main__":
    # get download url for latest release
    res = requests.get("https://api.github.com/repos/casheww/RW-RegionCast/releases/latest")
    download_url = res.json()["assets"][0]["url"]
    # download latest build
    download = requests.get(download_url, headers={"Accept": "application/octet-stream"})
    
    zip_buffer = BytesIO()
    zip_buffer.write(download.content)
    zip_buffer.seek(0)

    # read & hash sideapp exe from zip download
    with ZipFile(zip_buffer) as z:
        exe_bytes = z.read("RegionCast-DiscordGameSDK/RCApp.exe")
    new_hash = hashlib.md5(exe_bytes)
    print(new_hash.hexdigest())

    with open("exe-hash", "w+") as f:
        f.write(new_hash.hexdigest())
