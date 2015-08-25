#include "inc/zqtertiarybook.h"

ZQTertiaryBook::ZQTertiaryBook()
{
}

int ZQTertiaryBook::CreateBook(const QString& aPath)
{
    return -1;
}

ZQMove ZQTertiaryBook::FindNextMove(const ZQMove& aLastMove)
{
    ZQMove move = {0, false, 0, 0};
    return move;
}

ZQBook* ZQTertiaryBook::LoadBook(const QString& aPath)
{
    return NULL;
}
