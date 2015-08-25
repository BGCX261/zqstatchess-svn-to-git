using System;
using ZNStatChess.Types;
using ZNStatChess.Books;

namespace ZNStatChess.Runtime
{
    class ZNProgramArguments
    {
        static ZNProgramArguments singleton;

        string primaryBookPath;
        string secondaryBookPath;
        string tertiaryBookPath;
        int hashTableSize;
        ZNMakeBookInformation? typeOfBookForCreating = null;

        static ZNProgramArguments()
        {
            singleton = new ZNProgramArguments();
        }

        private ZNProgramArguments()
        {}

        static internal bool IsArgumentSet(ZNProgramArgumentsEnum arg)
        {
            bool returnValue = false; 
            switch (arg)
            {
                case ZNProgramArgumentsEnum.EPbook:
                    returnValue = singleton.primaryBookPath != null;
                    break;
                case ZNProgramArgumentsEnum.ESbook:
                    returnValue = singleton.secondaryBookPath != null;
                    break;
                case ZNProgramArgumentsEnum.ETbook:
                    returnValue = singleton.tertiaryBookPath != null;
                    break;
                case ZNProgramArgumentsEnum.EHash:
                    returnValue = singleton.hashTableSize != 0;
                    break;
                case ZNProgramArgumentsEnum.EMbook:
                    returnValue = singleton.typeOfBookForCreating != null;
                    break;
            }
            return returnValue;
        }

        static internal object GetArgument(ZNProgramArgumentsEnum argumentType)
        {
            object returnObject = null;
            switch (argumentType)
            {
                case ZNProgramArgumentsEnum.EPbook:
                    returnObject = singleton.primaryBookPath;
                    break;
                case ZNProgramArgumentsEnum.ESbook:
                    returnObject = singleton.secondaryBookPath;
                    break;
                case ZNProgramArgumentsEnum.ETbook:
                    returnObject = singleton.tertiaryBookPath;
                    break;
                case ZNProgramArgumentsEnum.EHash:
                    returnObject = singleton.hashTableSize;
                    break;
                case ZNProgramArgumentsEnum.EMbook:
                    returnObject = singleton.typeOfBookForCreating;
                    break;
            }
            return returnObject;
        }

        static internal void SetArgument(ZNProgramArgumentsEnum argumentType, object arg)
        {
            switch (argumentType)
            {
                case ZNProgramArgumentsEnum.EPbook:
                    singleton.primaryBookPath = arg as string;
                    break;
                case ZNProgramArgumentsEnum.ESbook:
                    singleton.secondaryBookPath = arg as string;
                    break;
                case ZNProgramArgumentsEnum.ETbook:
                    singleton.tertiaryBookPath = arg as string;
                    break;
                case ZNProgramArgumentsEnum.EHash:
                    try
                    {
                        singleton.hashTableSize = Convert.ToInt32(arg);
                    }
                    catch
                    {
                        singleton.hashTableSize = 0;
                    }
                    break;
                case ZNProgramArgumentsEnum.EMbook:
                    singleton.typeOfBookForCreating = arg as ZNMakeBookInformation?;
                    break;
                default:
                    break;
            }
        }
    }
}
