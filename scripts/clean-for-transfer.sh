#!/bin/sh
# Очистка исходников MailGate от временных файлов и артефактов сборки
# перед переносом в закрытый контур.
#
# Использование:
#   ./scripts/clean-for-transfer.sh          # показать, что будет удалено (dry-run)
#   ./scripts/clean-for-transfer.sh --apply  # удалить
set -eu

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
APPLY=0
[ "${1:-}" = "--apply" ] && APPLY=1

echo "Корень репозитория: $ROOT"
if [ "$APPLY" -eq 1 ]; then
    echo "Режим: УДАЛЕНИЕ"
else
    echo "Режим: dry-run (ничего не удаляется, запустите с --apply)"
fi
echo

# --- Каталоги с артефактами сборки и зависимостями ---
# .NET: bin, obj, publish, TestResults
# Node: node_modules, dist, кэши vite
# IDE/OS: .vs, .idea, .DS_Store
DIRS_TO_DELETE=$(find "$ROOT" \
    -type d \( \
        -name bin -o \
        -name obj -o \
        -name publish -o \
        -name TestResults -o \
        -name coverage -o \
        -name node_modules -o \
        -name dist -o \
        -name .vite -o \
        -name .vs -o \
        -name .idea \
    \) -prune 2>/dev/null | sort)

# --- Отдельные временные файлы ---
FILES_TO_DELETE=$(find "$ROOT" \
    \( -path "*/node_modules/*" -o -path "*/bin/*" -o -path "*/obj/*" \) -prune -o \
    -type f \( \
        -name ".DS_Store" -o \
        -name "Thumbs.db" -o \
        -name "*.log" -o \
        -name "*.trx" -o \
        -name "*.coveragexml" -o \
        -name "*.user" -o \
        -name "*.suo" -o \
        -name "*.tsbuildinfo" \
    \) -print 2>/dev/null | sort)

TOTAL_SIZE=0
COUNT=0

delete_path() {
    path="$1"
    size=$(du -sk "$path" 2>/dev/null | cut -f1 || echo 0)
    TOTAL_SIZE=$((TOTAL_SIZE + size))
    COUNT=$((COUNT + 1))
    printf '  %8s KB  %s\n' "$size" "${path#"$ROOT"/}"
    if [ "$APPLY" -eq 1 ]; then
        rm -rf "$path"
    fi
}

echo "--- Каталоги ---"
if [ -n "$DIRS_TO_DELETE" ]; then
    IFS='
'
    for dir in $DIRS_TO_DELETE; do
        delete_path "$dir"
    done
    unset IFS
else
    echo "  (нет)"
fi

echo
echo "--- Файлы ---"
if [ -n "$FILES_TO_DELETE" ]; then
    IFS='
'
    for file in $FILES_TO_DELETE; do
        delete_path "$file"
    done
    unset IFS
else
    echo "  (нет)"
fi

echo
if [ "$APPLY" -eq 1 ]; then
    echo "Удалено объектов: $COUNT, освобождено: $((TOTAL_SIZE / 1024)) MB"
else
    echo "Будет удалено объектов: $COUNT, освободится: $((TOTAL_SIZE / 1024)) MB"
    echo "Для удаления запустите: $0 --apply"
fi

# Напоминание про секреты
if [ -f "$ROOT/.env" ]; then
    echo
    echo "ВНИМАНИЕ: в корне найден .env — проверьте, что секреты не должны попасть в закрытый контур."
fi
