#include <QFile>
#include <iostream>
#include <QTextStream>

#include "inc/zqprimarybook.h"
#include "inc/zqmovesdatabase.h"

//#define NODS 4
//#define DEPTH 28
#define NODS 4
#define DEPTH 4

ZQPrimaryBook::ZQPrimaryBook()
{
}

int ZQPrimaryBook::CreateBook(const QString& aPath)
{
    return MakeBook(aPath, DEPTH, NODS);
}

int ZQPrimaryBook::MakeBook(QString aInputPath, int aDepth, int aNods)
    {
    std::cerr << "ZQPrimaryBook::MakeBook: path:" << aInputPath.toAscii().data() << ", depth: " << aDepth << ", nods per parent: " << aNods << "\n";

//    QString newPath(aPath);
//    QFile outputFile(newPath.append(".bin"));

//    std::cerr << "Output file: " << newPath.toAscii().data() << "\n";

//    if (!outputFile.open(QFile::WriteOnly|QFile::Truncate))
//    {
//        return 1;
//    }

//    std::cerr << "not return \n";

//    quint16* toWrite = new quint16(1);
//    for (int i = 0; i < DEPTH; i++)
//    {
//        for (int j = 0; j < i*NODS; j++)
//        {
//            *toWrite = (j % DEPTH) << i;
//            outputFile.write((char *)(toWrite));
//            std::cerr << *toWrite << " ";
//        }
//        std::cerr << "\n";
//    }

//    outputFile.close();
//    return 0;

    QFile inputFile(aInputPath);
    QFile outputFile(aInputPath.append(".bin"));

    if (!inputFile.open(QFile::ReadOnly|QFile::Text))
    {
        std::cerr << "Error during opening input file: " << aInputPath.data() << "\n";
        return 1;
    }

    if (!outputFile.open(QFile::WriteOnly|QFile::Truncate))
    {
        std::cerr << "Error during opening output file: " << aInputPath.append(".bin").data() << "\n";
        return 1;
    }

    ZQMovesDatabase allMoves = GetMovesFromPGNFile(inputFile, aDepth);
    inputFile.close();

    ZQMovesDatabase goodMoves = FindMovesForBook(allMoves, aNods);
    allMoves.Clean();

    SaveBook(goodMoves, outputFile);
    goodMoves.Clean();
    outputFile.close();

    return 0;
}

ZQMovesDatabase ZQPrimaryBook::GetMovesFromPGNFile(QFile &aInputFile, int aDepth)
{
    ZQMovesDatabase returnValue();
    QTextStream inStream(&aInputFile);
    bool wholeGameAnalysed;
    QString line();

    while (true)
    {
        wholeGameAnalysed = false;
        while (!wholeGameAnalysed)
        {

        }
    }

    return returnValue();
}

ZQMovesDatabase ZQPrimaryBook::FindMovesForBook(ZQMovesDatabase &aInputDatabase, int aNodes)
{
    return ZQMovesDatabase();
}

int ZQPrimaryBook::SaveBook(ZQMovesDatabase &aMoves, QFile &aFile)
{
    return 0;
}

ZQMove ZQPrimaryBook::FindNextMove(const ZQMove& aLastMove)
{
    ZQMove move = {0, false, 0, 0};
    return move;
}

ZQBook* ZQPrimaryBook::LoadBook(const QString& aPath)
{
    return NULL;
}
