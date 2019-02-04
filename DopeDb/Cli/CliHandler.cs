using DopeDb.Shared.Cli;
using DopeDb.Shared.Util;
using DopeDb.Shared.Reflection;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;

namespace DopeDb.Cli {
    class CliHandler {
        public void HandleCliCommand(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("No arguments given");
                System.Environment.Exit(1);
            }
            try {
                var cliCall = ParseCliCall(args);
                InvokeCliCall(cliCall);
            }
            catch (System.ArgumentException e)
            {
                Util.WriteLine(e.Message);
                Util.WriteLine("Call help:list to see all available commands");
            }
            catch (System.Exception e)
            {
                Util.WriteException(e);
            }
        }

        public void InvokeCliCall(CliCall cliCall)
        {
            var commandName = $"{cliCall.ControllerName}:{cliCall.ActionName}";
            var command = GetCommand(commandName);
            if (command == null)
            {
                Util.WriteLine($"<error>Command {commandName} is unknown</error>");
                Util.WriteLine("Call help:list to see all available commands");
                return;
            }
            var commandController = System.Activator.CreateInstance(command.DeclaringType);
            var arguments = MapArguments(command, cliCall);
            var request = new CliRequest();
            request.ActionName = cliCall.ActionName;
            request.ControllerName = cliCall.ControllerName;
            request.Arguments = new Dictionary<string, object>();
            foreach (var parameter in command.GetParameters())
            {
                request.Arguments.Add(parameter.Name, arguments[parameter.Position]);
            }
            ((AbstractCliController)commandController).SetCliReqest(request);
            var actionName = Text.FirstCharToUpper(cliCall.ActionName) + "Command";
            command.Invoke(commandController, arguments);
        }

        protected object[] MapArguments(MethodInfo actionInfo, CliCall cliCall)
        {
            var result = new List<object>();
            var consumedParameters = 0;
            foreach (var parameter in actionInfo.GetParameters())
            {
                var targetName = parameter.Name;
                object[] argumentString = null;
                object targetValue = null;
                if (cliCall.HasArgument(targetName))
                {
                    argumentString = cliCall.GetArgument(targetName);
                    System.Console.WriteLine(argumentString);
                }
                else if (cliCall.HasArgument(consumedParameters))
                {
                    argumentString = new object[1]{ cliCall.GetArgument(consumedParameters++) };
                }
                else if (parameter.IsOptional && parameter.HasDefaultValue)
                {
                    targetValue = parameter.DefaultValue;
                }
                else
                {
                    throw new System.ArgumentException($"Missing parameter {targetName}.");
                }
                if (targetValue == null)
                {
                    // TODO: map properties
                    targetValue = argumentString.Length > 0 ? argumentString[0].ToString() : string.Empty;
                }
                result.Add(targetValue);
            }
            return result.ToArray();
        }

        protected CliCall ParseCliCall(string[] args)
        {
            var cliCall = new CliCall();
            if (args.Length < 1 || args[0].IndexOf(':') < 0)
            {
                throw new System.ArgumentException("Not a valid command");
            }
            var commandParts = args[0].Split(':');
            cliCall.ControllerName = commandParts[0].ToLower();
            cliCall.ActionName = commandParts[1].ToLower();
            var namedArguments = new Dictionary<string, List<string>>();
            var positionalArguments = new List<string>();
            string currentArgument = null;
            void addArgument (string n, string v)
            {
                if (!namedArguments.ContainsKey(n))
                {
                    namedArguments.Add(n, new List<string>());
                }
                namedArguments[n].Add(v);
            };
            for (var i = 1; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg[0] == '-')
                {
                    if (currentArgument != null)
                    {
                        addArgument(currentArgument, "true");
                    }
                    if (arg[1] == '-')
                    {
                        currentArgument = arg.Substring(2);
                    }
                    else
                    {
                        for (var j = 1; j < arg.Length - 1; j++)
                        {
                            addArgument(arg[j].ToString(), "true");
                        }
                        currentArgument = arg[arg.Length - 1].ToString();
                    }
                }
                else
                {
                    if (currentArgument == null)
                    {
                        positionalArguments.Add(arg);
                    }
                    else {
                        addArgument(currentArgument, arg);
                        currentArgument = null;
                    }
                }
            }
            cliCall.PositionalArguments = positionalArguments;
            cliCall.NamedArguments = namedArguments;
            return cliCall;
        }

        public Dictionary<string, MethodInfo> GetAllCommands()
        {
            var result = new Dictionary<string, MethodInfo>();
            var cliControllers = Reflection.Instance().GetSubclasses(typeof(AbstractCliController));
            foreach (var cliController in cliControllers)
            {
                var controllerShortName = cliController.Name.Replace(".Cli.", ".").Replace("CliController", "").ToLower();
                var methodInfos = cliController.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var methodInfo in methodInfos)
                {
                    if (!methodInfo.Name.EndsWith("Command"))
                    {
                        continue;
                    }
                    var actionName = methodInfo.Name.Substring(0, methodInfo.Name.Length - 7).ToLower();
                    var commandName = $"{controllerShortName}:{actionName}";
                    result.Add(commandName, methodInfo);
                }
            }
            return result;
        }

        public MethodInfo GetCommand(string commandName)
        {
            var commands = GetAllCommands();
            if (!commands.ContainsKey(commandName))
            {
                return null;
            }
            return commands[commandName];
        }
    }
}
