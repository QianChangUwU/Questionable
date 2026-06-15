<section id="top">
    <p style="text-align:center;" align="center">
        <img align="center" src="https://github.com/qstxiv/icons/raw/main/Questionable.png" width="250" />
    </p>
    <h1 style="text-align:center;" align="center">Questionable (CN)</h1>
    <p style="text-align:center;" align="center">
        国服适配的任务自动化插件，带完整汉化。
    </p>
</section>

<!-- Badges -->
<p align="center"> 
  <a href="https://github.com/QianChangUwU/Questionable/commits/new-main" alt="Commits">
    <img src="https://img.shields.io/github/last-commit/QianChangUwU/Questionable/new-main?color=00D162&style=for-the-badge" /></a>
   <a href="https://github.com/QianChangUwU/Questionable/commits/new-main" alt="Commit Activity">
    <img src="https://img.shields.io/github/commit-activity/m/QianChangUwU/Questionable?color=00D162&style=for-the-badge" /></a>
  <br> 
  <a href="https://github.com/QianChangUwU/Questionable/issues" alt="Open Issues">
    <img src="https://img.shields.io/github/issues-raw/QianChangUwU/Questionable?color=EA9C0A&style=for-the-badge" /></a>
  <a href="https://github.com/QianChangUwU/Questionable/graphs/contributors" alt="Contributors">
    <img src="https://img.shields.io/github/contributors/QianChangUwU/Questionable?color=009009&style=for-the-badge" /></a>
<br>
  <a href="https://github.com/QianChangUwU/Questionable/tags" alt="Release">
    <img src="https://img.shields.io/github/v/tag/QianChangUwU/Questionable?label=Release&logo=git&logoColor=ffffff&style=for-the-badge" /></a>
</p>

<section id="contents">

### Contents
* [关于](#about)
* [安装](#installation)
* [命令](#commands)
* [使用说明](#usage)

</section>

<section id="about">

# 关于

<p> Questionable 是一款基于 <a href="https://github.com/goatcorp/Dalamud">Dalamud</a> 的第三方插件，支持国际服与国服。<br><br>
    本仓库是基于 <a href="https://github.com/PunishXIV/Questionable">PunishXIV/Questionable</a> 的国服适配分支，主要改动包括：
    <ul>
    <li>完整的中文界面汉化</li>
    <li>国服 Dalamud 环境适配（Dalamud.CN.NET.Sdk）</li>
    <li>任务路径数据源指向国服仓库</li>
    </ul>
</p>

</section><br>

<!-- Installation -->
<section id="installation"><br>

# 安装

### 方法一：通过 Dalamud 插件仓库安装

1. 打开游戏内 Dalamud 设置（`/xlsettings`）
2. 在"自定义插件仓库"中填入：<br>
   <code>https://raw.githubusercontent.com/QianChangUwU/DalamudPlugins/main/pluginmaster.json</code>
3. 点击 "+" 添加，然后保存关闭
4. 打开 Dalamud 插件安装器（`/xlplugins`）
5. 在"所有插件"中搜索 "Questionable" 并安装

### 方法二：手动安装

从 <a href="https://github.com/QianChangUwU/Questionable/releases">Releases</a> 下载最新版本，解压到 <code>%AppData%\XIVLauncherCN\addon\Hooks\dev\</code> 目录下。

</section><br>

<!-- Commands -->
<section id="commands">

# 命令

<table>
<thead>
<tr>
<th align="left"><strong>聊天命令</strong></th>
<th align="left"><strong>功能</strong></th>
</tr>
</thead>
<tbody>
<tr>
<td align="left"><code>/qst</code></td>
<td align="left">打开任务窗口</td>
</tr>
<tr>
<td align="left"><code>/qst config</code></td>
<td align="left">打开设置窗口</td>
</tr>
<tr>
<td align="left"><code>/qst start</code></td>
<td align="left">开始执行任务</td>
</tr>
<tr>
<td align="left"><code>/qst stop</code></td>
<td align="left">停止执行任务</td>
</tr>
<tr>
<td align="left"><code>/qst reload</code></td>
<td align="left">重新加载所有任务数据</td>
</tr>
<tr>
<td align="left"><code>/qst which</code></td>
<td align="left">显示当前选中目标相关的所有任务</td>
</tr>
<tr>
<td align="left"><code>/qst zone</code></td>
<td align="left">显示当前区域可接的任务</td>
</tr>
</tbody>
</table>

</section><br>

<!-- Usage -->
<section id="usage"><br>

# 使用说明

### 前置插件

本插件需要配合以下插件使用：

- <b>[vnavmesh](https://github.com/awgil/ffxiv_navmesh)</b> — 自动寻路
- <b>[TextAdvance](https://github.com/NightmareXIV/TextAdvance)</b> — 自动对话/过场
- <b>[Lifestream](https://github.com/NightmareXIV/Lifestream)</b> — 自动传送

### 可选插件

- <b>[Boss Mod (VBM)](https://github.com/awgil/ffxiv_bossmod)</b> / <b>[Wrath Combo](https://github.com/PunishXIV/WrathCombo)</b> / <b>[Rotation Solver Reborn](https://github.com/FFXIV-CombatReborn/RotationSolverReborn)</b> — 自动战斗
- <b>[Artisan](https://github.com/PunishXIV/Artisan)</b> — 自动制作
- <b>[AutoDuty](https://github.com/ffxivcode/AutoDuty)</b> — 自动副本

</section><br>

<!-- Credits -->
<section id="credits">

# 致谢

本 fork 基于 [PunishXIV/Questionable](https://github.com/PunishXIV/Questionable) 上游仓库，原作由 <a href="https://github.com/carvelli">Liza Carvelli</a> 创作，<a href="https://puni.sh/">Puni.sh</a> 团队维护。

原项目贡献者：
- alydev、Kiarra、Kage、Jaksuhn、erdelf、Limiana

</section><br>
