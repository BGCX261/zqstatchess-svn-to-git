#ifndef ZQTERTIARYBOOK_H
#define ZQTERTIARYBOOK_H

#include "inc/zqbook.h"

class ZQTertiaryBook : public ZQBook
{
public:
    static int CreateBook(const QString& aPath);
    ZQMove FindNextMove(const ZQMove& aLastMove);
    ZQBook* LoadBook(const QString& aPath);
protected:
    ZQTertiaryBook();
};

#endif // ZQTERTIARYBOOK_H
