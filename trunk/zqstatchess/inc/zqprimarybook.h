#ifndef ZQPRIMARYBOOK_H
#define ZQPRIMARYBOOK_H

#include "inc/zqbook.h"

class ZQMovesDatabase;

class ZQPrimaryBook : public ZQBook
{
public:
    static int CreateBook(const QString& aPath);
    virtual ZQMove FindNextMove(const ZQMove& aLastMove);
    virtual ZQBook* LoadBook(const QString& aPath);

protected:
    ZQPrimaryBook();
    quint32 iIndex;

private:
    static int MakeBook(QString aInputPath, int aDepth, int aNods);
    static ZQMovesDatabase GetMovesFromPGNFile(QFile &aInputFile, int aDepth);
    static ZQMovesDatabase FindMovesForBook(ZQMovesDatabase &aInputDatabase, int aNodes);
    static int SaveBook(ZQMovesDatabase &aMoves, QFile &aFile);
};

#endif // ZQPRIMARYBOOK_H
