# 00 Final Report Writing Guide / 00 最终报告写作指南

This file explains how to turn the split GitHub-readable documentation into the final Word or PDF submission.

本文件说明如何将当前适合 GitHub 阅读的分章节文档，整理为最终提交的 Word 或 PDF 报告。

## Recommended Final Report Order / 建议的最终报告顺序

1. Introduction / 引言
2. Project Overview / 项目概述
3. System Overview / 系统总览
4. System Architecture / 系统架构
5. Raspberry Pi and Hardware Documentation / 树莓派与硬件文档
6. Unity Environment Reconstruction Guide / Unity 环境重建指南
7. Unity Scripts and Inspector Guide / Unity 脚本与 Inspector 指南
8. Installation and Deployment Guide / 安装与部署指南
9. Operation Manual / 操作手册
10. Testing and Validation / 测试与验证
11. Troubleshooting / 故障排查
12. Known Issues and Limitations / 已知问题与限制
13. GitHub Repository and References / GitHub 仓库与参考资料
14. Appendix / 附录

## Source Mapping / 内容来源映射

| Final report section | Main source file |
| --- | --- |
| Introduction, Project Overview, System Overview, System Architecture | `docs/TECHNICAL_DOCUMENTATION_REVISED_BILINGUAL.md` |
| Raspberry Pi and Hardware Documentation | `docs/readable-bilingual/02-raspberry-pi.md` |
| Unity Environment Reconstruction Guide | `docs/readable-bilingual/03-unity-environment.md` |
| Unity Scripts and Inspector Guide | `docs/readable-bilingual/04-unity-scripts-and-inspector.md` |
| Installation and Deployment Guide | `docs/readable-bilingual/05-setup-and-operation.md` |
| Operation Manual | `docs/readable-bilingual/05-setup-and-operation.md` |
| Testing and Troubleshooting | `docs/readable-bilingual/06-testing-and-troubleshooting.md` |
| GitHub Repository and References | `docs/readable-bilingual/07-handover-and-links.md` |

| 最终报告章节 | 主要来源文件 |
| --- | --- |
| 引言、项目概述、系统总览、系统架构 | `docs/TECHNICAL_DOCUMENTATION_REVISED_BILINGUAL.md` |
| 树莓派与硬件文档 | `docs/readable-bilingual/02-raspberry-pi.md` |
| Unity 环境重建指南 | `docs/readable-bilingual/03-unity-environment.md` |
| Unity 脚本与 Inspector 指南 | `docs/readable-bilingual/04-unity-scripts-and-inspector.md` |
| 安装与部署指南 | `docs/readable-bilingual/05-setup-and-operation.md` |
| 操作手册 | `docs/readable-bilingual/05-setup-and-operation.md` |
| 测试与故障排查 | `docs/readable-bilingual/06-testing-and-troubleshooting.md` |
| GitHub 仓库与参考资料 | `docs/readable-bilingual/07-handover-and-links.md` |

## Recommended Figure Placement / 建议插图位置

| Figure | Recommended section | Suggested content |
| --- | --- | --- |
| Figure 1 | System Architecture | overall system flow |
| Figure 2 | Raspberry Pi and Hardware | Raspberry Pi plus breadboard overview |
| Figure 3 | Raspberry Pi and Hardware | joystick, slider, and encoder mapping |
| Figure 4 | Raspberry Pi and Hardware | MCP3008 analog conversion flow |
| Figure 5 | Unity Environment | Unity project and main scene overview |
| Figure 6 | Unity Scripts and Inspector | `PiSystemBridge` Inspector screenshot |
| Figure 7 | Setup and Operation | SSH and script execution example |
| Figure 8 | Testing and Validation | terminal output and Unity Console evidence |
| Figure 9 | GitHub Repository | repository homepage or docs folder screenshot |

| 图号 | 建议章节 | 图示内容 |
| --- | --- | --- |
| 图 1 | 系统架构 | 系统总体流程图 |
| 图 2 | 树莓派与硬件 | 树莓派和面包板整体图 |
| 图 3 | 树莓派与硬件 | 摇杆、滑杆和旋钮映射图 |
| 图 4 | 树莓派与硬件 | MCP3008 模拟量转换流程图 |
| 图 5 | Unity 环境 | Unity 项目和主场景总览图 |
| 图 6 | Unity 脚本与 Inspector | `PiSystemBridge` Inspector 截图 |
| 图 7 | 配置与操作 | SSH 和脚本运行示例图 |
| 图 8 | 测试与验证 | 终端输出和 Unity Console 验证图 |
| 图 9 | GitHub 仓库 | 仓库首页或 docs 目录截图 |

## Recommended Table Placement / 建议表格位置

| Table | Recommended section | Suggested content |
| --- | --- | --- |
| Table 1 | Raspberry Pi and Hardware | hardware list |
| Table 2 | Raspberry Pi and Hardware | pin and channel mapping |
| Table 3 | Unity Scripts and Inspector | script purpose and attachment |
| Table 4 | Setup and Operation | command and platform mapping |
| Table 5 | Testing and Troubleshooting | common issue checklist |

| 表号 | 建议章节 | 表格内容 |
| --- | --- | --- |
| 表 1 | 树莓派与硬件 | 硬件清单 |
| 表 2 | 树莓派与硬件 | 引脚与通道映射 |
| 表 3 | Unity 脚本与 Inspector | 脚本用途与挂载位置 |
| 表 4 | 配置与操作 | 命令与平台对应关系 |
| 表 5 | 测试与故障排查 | 常见问题检查表 |

## Writing Strategy / 写作策略

**English**
- Use the full revised document for complete wording.
- Use the split readable files when you want cleaner structure and shorter paragraphs.
- In the final Word document, merge them into one continuous report.
- Keep screenshots and figure captions simple and factual.

**中文**
- 需要完整表述时，以完整版文档为主。
- 需要更清晰结构和更短段落时，以分章节可读版为主。
- 在最终 Word 文档中，将这些内容整合为一份连续报告。
- 截图和图注尽量保持简洁、客观。

## Practical Next Step / 现在最实际的下一步

**English**  
If you are writing the final report yourself, start from the split readable files and only copy longer explanation from the full revised document when needed.

**中文**  
如果你要自己整理最终报告，建议先以分章节可读版为主写，再在需要时从完整版文档中补充较长说明。
