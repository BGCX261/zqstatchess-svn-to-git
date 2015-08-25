#ifndef ZQTYPES_H
#define ZQTYPES_H

#include <qmath.h>

struct ZQMove
{
    quint16 iMove;
    bool iIsBookMove;
    quint16 iRating;
    quint16 iPly;
};

enum ZQBookTypes
{
    EPrimary = 1,
    ESecondary,
    ETercialy
};

#endif // ZQTYPES_H
