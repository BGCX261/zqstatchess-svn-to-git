using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZNStatChess.Types;
using ZNStatChess.Books;
using System.Threading;
using ZNStatChess.ChessEngine;

namespace ZNStatChess.Runtime
{
    static class ZNProgram
    {
        static int Main(string[] args)
        {
            int i = 0;

            if (args.Length == 0)
            {
                PrintHelp();
                return ZNConstants.EXIT_SUCCESS;
            }

            while (i < args.Length)
            {
                i++;
                string arg = args[i].ToLower();
                arg.Replace('/', '-');
                if (arg.Equals("-help"))
                {
                    PrintHelp();
                    if (arg.Length == 1)
                    {
                        return ZNConstants.EXIT_SUCCESS;
                    }
                    return ZNConstants.EXIT_FAILURE;
                }
                else if (arg.Equals("-mbook"))
                {
                    ZNProgramArguments.SetArgument(ZNProgramArgumentsEnum.EMbook, args[++i]+"\n"+args[++i]);
                }
                else if (arg.Equals("-hash"))
                {
                    ZNProgramArguments.SetArgument(ZNProgramArgumentsEnum.EHash, args[++i]);
                }
                else if (arg.Equals("-pbbok"))
                {
                    ZNProgramArguments.SetArgument(ZNProgramArgumentsEnum.EPbook, args[++i]);
                }
                else if (arg.Equals("-sbbok"))
                {
                    ZNProgramArguments.SetArgument(ZNProgramArgumentsEnum.ESbook, args[++i]);
                }
                else if (arg.Equals("-tbbok"))
                {
                    ZNProgramArguments.SetArgument(ZNProgramArgumentsEnum.ETbook, args[++i]);
                }
            }

            if (ZNProgramArguments.IsArgumentSet(ZNProgramArgumentsEnum.EPbook))
            {
                if (ZNProgramArguments.IsArgumentSet(ZNProgramArgumentsEnum.EMbook))
                {
                    Console.WriteLine("You can't specify both -pbook and -mbook options!\n\n");
                    return ZNConstants.EXIT_FAILURE;
                }

                ZNChessEngine engine = new ZNChessEngine(ZNProgramArguments.GetArgument(ZNProgramArgumentsEnum.EHash) as int?,
                    ZNProgramArguments.GetArgument(ZNProgramArgumentsEnum.EPbook) as string,
                    ZNProgramArguments.GetArgument(ZNProgramArgumentsEnum.ESbook) as string,
                    ZNProgramArguments.GetArgument(ZNProgramArgumentsEnum.ETbook) as string);

                // TODO Better synchronization.
                engine.Go.BeginInvoke(null, null);
                engine.Go.EndInvoke(null);

                return ZNConstants.EXIT_SUCCESS;
            }

            if (ZNProgramArguments.IsArgumentSet(ZNProgramArgumentsEnum.EMbook))
            {
                ZNMakeBookInformation? whatMakeNullable = ZNProgramArguments.GetArgument(ZNProgramArgumentsEnum.EMbook)  as ZNMakeBookInformation?;
                if (whatMakeNullable != null)
                {
                    ZNMakeBookInformation whatMake = (ZNMakeBookInformation)whatMakeNullable;
                    return CreateBook(whatMake.inputFilePath, whatMake.type);
                }
                Console.WriteLine("Error in input arguments for -mbook.");    
            }

            PrintHelp();
            return ZNConstants.EXIT_FAILURE;
        }

        static void PrintHelp()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("       ZQStatChess -pbook <primaly book> [-sbook <secondary book>] [-tbook <tertialy book>] -hash <size_in_MB>\n");
            Console.WriteLine("            (e. g.: ZQStatChess -pbook pb.bin -tbook tb.bin -hash 32)]\n\n");
            Console.WriteLine("       ZQStatChess -mbook <PGN input> <type: {1, 2, 3}> (e. g.: ZQStatChess -mbook my.pgn 1)\n\n");
            Console.WriteLine("       ZQStatChess -help\n\n");
        }

        static int CreateBook(string aPath, ZNBookTypes aType)
        {
            int returnCode = -1;
            switch (aType)
            {
                case ZNBookTypes.EPrimary:
                    returnCode = ZNPrimaryBook.Instance.CreateBook(aPath);
                    break;
                case ZNBookTypes.ESecondary:
                    returnCode = ZNSecondaryBook.Instance.CreateBook(aPath);
                    break;
                case ZNBookTypes.ETercialy:
                    returnCode = ZNTertiaryBook.Instance.CreateBook(aPath);
                    break;
            }
            return returnCode;
        }
    }
}
