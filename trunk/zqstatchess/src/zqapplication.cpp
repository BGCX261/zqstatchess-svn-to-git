#include <iostream>
#include <cstdlib>
#include <QtDebug>

#include "inc/zqapplication.h"
#include "inc/zqbookfactory.h"

ZQApplication::ZQApplication(int &aArgc, char **aArgv) :
    QCoreApplication(aArgc, aArgv), iArgc(aArgc), iArgv(aArgv)
{
}

int ZQApplication::exec()
{
    int i = 1, tmp = 0;
    char *p;

    while (i < iArgc) {
      for (p = iArgv[i]; *p; p++) *p = tolower (*p);
      if (!strcmp ("-help", iArgv[i]) || !strcmp ("/help", iArgv[i])) {
          PrintHelp();
          return(EXIT_SUCCESS);
        }

      // TO DO Tady je chyba, je treba odlisit tvorbu knhy zahajeni a hrani.
      if (!strcmp ("-hash", iArgv[i]) || !strcmp ("/hash", iArgv[i])) {
        if (++i >= iArgc) {
            std::cerr << "You must specify a hash size: faile -hash <size>\n";
      return(EXIT_FAILURE);
        }
        else {
      hash_max_mb = (quint32)atol(iArgv[i++]);
      if (atol (iArgv[i-1]) < 0) {
        std::cerr << "Hash table size must be positive!\n";
        return(EXIT_FAILURE);
      }
        }
      }
      else if (!strcmp ("-mbook", iArgv[i]) || !strcmp ("/mbook", iArgv[i])) {
        if (++i >= iArgc) {
      std::cerr << "You must specify an input file.\n";
      PrintHelp();
      return(EXIT_FAILURE);
        }
        if (++i >= iArgc) {
      /* no type specified */
      std::cerr << "No type given.\n";
      PrintHelp();
      return(EXIT_FAILURE);
        }
        else {
      /* max ply is specified */
      tmp = atoi (iArgv[i]);
      if ((tmp < EPrimary) || (tmp > ETercialy)){
        std::cerr << "Wrong type!\n";
        PrintHelp();
        return(EXIT_FAILURE);
      }
      make_book(iArgv[i-1], tmp);
      i++;
        }
      }
      else {
        std::cerr << "\nUnrecognized command line argument: " << iArgv[i++] <<"\n\n";
        PrintHelp();
        return(EXIT_FAILURE);
      }
    }
return(EXIT_SUCCESS);
}

int ZQApplication::make_book(char *path, int tmp)
{
    std::cout << "I am creating.";
    return(ZQBookFactory::CreateBook(QString(path), tmp));
}

void ZQApplication::PrintHelp()
{
    std::cout << "Usage:\n";
    std::cout << "       ZQStatChess -pbook <primaly book> [-sbook <secondary book>] [-tbook <tertialy book>] -hash <size_in_MB>\n";
    std::cout << "            (e. g.: ZQStatChess -pbook pb.bin -tbook tb.bin -hash 32)]\n\n";
    std::cout << "       ZQStatChess -mbook <PGN input> <type: {1, 2, 3}>  (e. g.: ZQStatChess -mbook my.pgn 1)\n\n";
    std::cout << "       ZQStatChess -help\n\n";
}

