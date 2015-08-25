#ifndef ZQBOOKFACTORY_H
#define ZQBOOKFACTORY_H

#include "inc/zqtypes.h"

class ZQBook;

class ZQBookFactory
{
public:
    static int CreateBook(const QString& aPath, int aType);
    static ZQBook* LoadBook(const QString& aPath, ZQBookTypes aType);
private:
    ZQBookFactory();
};

#endif // ZQBOOKFACTORY_H
