#include <QtCore/QCoreApplication>

#include "inc/zqapplication.h"

int main(int argc, char *argv[])
{
    ZQApplication a(argc, argv);

    return a.exec();
}
