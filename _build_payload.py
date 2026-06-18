import json, os

files_list = []
for root, dirs, filenames in os.walk('.'):
    if '.git' in root:
        continue
    for f in filenames:
        fp = os.path.join(root, f).replace('\\', '/')
        if fp.startswith('./'):
            fp = fp[2:]
        try:
            with open(os.path.join(root, f), 'r', encoding='utf-8') as fh:
                content = fh.read()
        except:
            continue
        files_list.append({"path": fp, "content": content})

with open('_push_payload.json', 'w', encoding='utf-8') as out:
    json.dump(files_list, out, ensure_ascii=False)

print(f"Total files to push: {len(files_list)}")
total = sum(len(f['content']) for f in files_list)
print(f"Total size: {total} bytes")
