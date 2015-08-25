#ifndef ZQAPPLICATION_H
#define ZQAPPLICATION_H

#define MAX_B_PLY 100

#include <QCoreApplication>

class ZQBook;

class ZQApplication : public QCoreApplication
{
    Q_OBJECT
public:
    ZQApplication(int &argc, char **argv);
    void PrintHelp();
    int exec();

private:
    int make_book(char *path, int tmp);

    quint32 hash_max_mb;
    QList<ZQBook*> iBooks;
    int iArgc;
    char** iArgv;

signals:

public slots:

};

#endif // ZQAPPLICATION_H
