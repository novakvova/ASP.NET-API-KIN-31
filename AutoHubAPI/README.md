## Налаштування Claude Code у проект ствимо черех PowerShell
```
irm https://claude.ai/install.ps1 | iex
Test-Path "$env:USERPROFILE\.local\bin\claude.exe"
[Environment]::SetEnvironmentVariable("Path", $env:Path + ";$env:USERPROFILE\.local\bin", "User")

claude --version

claude
/init
Проаналізуй структуру проєкту, технології,
залежності та основні архітектурні рішення та запиши у spec.md

Додай у CLAUDE.md, додай правила кодування, архітектури та тестування.

/cost

/model

claude --resume 
```