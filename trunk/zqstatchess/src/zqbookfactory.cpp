#include "inc/zqbookfactory.h"
#include "inc/zqprimarybook.h"
#include "inc/zqsecondarybook.h"
#include "inc/zqtertiarybook.h"

ZQBookFactory::ZQBookFactory()
{
}

int ZQBookFactory::CreateBook(const QString &aPath, int aType)
{
    switch (aType)
    {
    case EPrimary:
        ZQPrimaryBook::CreateBook(aPath);
        break;
    case ESecondary:
        ZQSecondaryBook::CreateBook(aPath);
        break;
    case ETercialy:
        ZQTertiaryBook::CreateBook(aPath);
        break;
    }
    return( -1);
}
