#ifndef ZQBOOK_H
#define ZQBOOK_H

#include "inc/zqtypes.h"

class QFile;
class QString;

class ZQBook
{
public:
    static int CreateBook(const QString& aPath) {return(-1);}
    virtual ZQMove FindNextMove(const ZQMove& aLastMove) {ZQMove move = {0, false, 0, 0}; return move;}
    virtual ZQBook* LoadBook(const QString& aPath) {return NULL;}
protected:
    QFile* iDataFile;
};

#endif // ZQBOOK_H
