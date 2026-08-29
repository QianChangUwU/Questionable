import json
import os
import sys

repo_path = sys.argv[1]
version = sys.argv[2]
repo_full_name = sys.argv[3]

json_path = os.path.join(repo_path, 'pluginmaster.json')

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

entry = data[0] if data else {}

download_url = f"https://github.com/{repo_full_name}/releases/download/v{version}/latest.zip"

entry['Name'] = 'Questionable'
entry['Author'] = 'liza, qstxiv, & various contributors & QianChang'
entry['Punchline'] = '一个小小的任务助手插件。'
entry['Description'] = '一个小小的任务助手插件，能在可能的情况下自动为你完成任务。使用导航网格自动行走到所有任务路径点，并尝试沿途自动完成所有步骤（不包括副本、单人职责和战斗）。\n\n并非所有任务都受支持，请查看GitHub仓库获取最新列表。\n\n所需插件：vnavmesh、TextAdvance、Lifestream'
entry['InternalName'] = 'Questionable'
entry['AssemblyVersion'] = version
entry['TestingAssemblyVersion'] = version
entry['DalamudApiLevel'] = 15
entry['TestingDalamudApiLevel'] = 15
entry['DownloadLinkInstall'] = download_url
entry['DownloadLinkUpdate'] = download_url
entry['DownloadLinkTesting'] = download_url
entry['RepoUrl'] = f'https://github.com/{repo_full_name}'
entry['IconUrl'] = download_url
entry['Tags'] = ['quests', 'msq']
entry['ApplicableVersion'] = 'any'
entry['LoadPriority'] = 0
entry['AcceptsFeedback'] = True
entry['LastUpdate'] = int(__import__('time').time())

if not data:
    data = [entry]
else:
    data[0] = entry

with open(json_path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=4, ensure_ascii=False)
