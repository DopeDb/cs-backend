using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DopeDb.Shared.Cli;

namespace DopeDb.Cli {
    class HelpCliController : AbstractCliController {

        [Description("Lists all available commands")]
        public void ListCommand()
        {
            var allCommands = (new CliHandler()).GetAllCommands();
            System.Type previousCommandController = null;
            foreach (KeyValuePair<string, MethodInfo> cmd in allCommands)
            {
                string commandName = cmd.Key;
                MethodInfo action = cmd.Value;
                var commandController = action.DeclaringType;
                if (previousCommandController != commandController) {
                    previousCommandController = commandController;
                    Util.WriteLine($"\n<fg=Yellow>{commandController.Name}</>");
                    Util.WriteLine("<fg=DarkGreen>==========================================================</>");
                }
                Util.WriteLine(commandName, 2);
                var descriptionMethodName = action.Name.Replace("Command", "Description");
                var descriptionMethod = commandController.GetMethod(descriptionMethodName);
                string description = getDescription(action);
                if (description.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Util.WriteLine(description, 4);
                    Console.ResetColor();
                }
            }
        }

        [Description("Shows detailed description of a command and it's parameters")]
        [Parameter("commandName", Description = "The command to get more information for")]
        public void DescribeCommand(string commandName)
        {
            MethodInfo command = (new CliHandler()).GetCommand(commandName);
            if (command == null)
            {
                Util.WriteLine($"<error>No command {commandName} was found</error>");
                return;
            }
            var parameters = command.GetParameters();
            Util.WriteLine($"<fg=DarkGrey>defined in {command.DeclaringType.Name}</>\n");
            Util.WriteLine($"<fg=Yellow>Usage</>");
            var usageStringBuilder = new StringBuilder(commandName);
            foreach (var parameter in parameters)
            {
                if (parameter.IsOptional)
                    usageStringBuilder.Append($" [{parameter.Name}]");
                else
                    usageStringBuilder.Append($" <{parameter.Name}>");
            }
            Util.WriteLine(usageStringBuilder.ToString(), 2);
            Util.WriteLine();
            
            var helpText = getHelpText(command);
            if (!string.IsNullOrEmpty(helpText))
            {
                Util.WriteLine($"<fg=Yellow>Help</>");
                Util.WriteLine(getHelpText(command));
                Util.WriteLine();
            }

            if (parameters.Length > 0)
            {
                Util.WriteLine($"<fg=Yellow>Arguments</>");
                var parameterAttributes = command.GetCustomAttributes(typeof(Parameter));
                var parameterDescriptions = new Dictionary<string, string>();
                foreach (var parameterAttribute in parameterAttributes)
                {
                    var attribute = (Parameter)parameterAttribute;
                    parameterDescriptions.Add(attribute.GetName(), attribute.Description);
                }
                foreach (var parameter in parameters)
                {
                    Util.Write($"<fg=White>--{CliCall.GetCliParameterName(parameter.Name)}</>", 2);
                    if (parameter.HasDefaultValue)
                        Util.Write($" <fg=DarkGrey>({parameter.DefaultValue.ToString()})</>", 2);
                    Util.WriteLine();
                    if (parameterDescriptions.ContainsKey(parameter.Name) && !string.IsNullOrEmpty(parameterDescriptions[parameter.Name]))
                        Util.WriteLine(parameterDescriptions[parameter.Name], 4);
                }
            }
        }

        protected string getDescription(MethodInfo action)
        {
            var descriptionMethodName = action.Name.Replace("Command", "Description");
            var descriptionMethod = action.DeclaringType.GetMethod(descriptionMethodName);
            string description = "";
            if (descriptionMethod != null && descriptionMethod.IsStatic)
            {
                description = (string)descriptionMethod.Invoke(null, null);
            }
            else
            {
                var descriptionAttribute = action.GetCustomAttribute(typeof(Description), false);
                if (descriptionAttribute != null && descriptionAttribute is Description)
                {
                    description = ((Description)descriptionAttribute).GetDescription();
                }
            }
            return description;
        }

        protected string getHelpText(MethodInfo action)
        {
            var helpMethodName = action.Name.Replace("Command", "Help");
            var helpMethod = action.DeclaringType.GetMethod(helpMethodName);
            string helpText = "";
            if (helpMethod != null && helpMethod.IsStatic)
            {
                helpText = (string)helpMethod.Invoke(null, null);
            }
            else
            {
                helpText = this.getDescription(action);
            }
            return helpText;
        }

        [Parameter("firstArg", Description="Nothing meaningful")]        
        public void TestCommand(string firstArg, int opt1 = 5, bool someFlag = false, string otherString = "foo")
        {
            Console.WriteLine(firstArg);
            Console.WriteLine(opt1);
            Console.WriteLine(someFlag);
            Console.WriteLine(otherString);
        }
    }
}