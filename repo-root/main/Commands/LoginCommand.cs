using System;
using RecipeManager.logs; 

namespace RecipeManager
{
    public class LoginCommand : ICommand
    {
        public string Name => "login";
        public bool RequiresLogin => false;

        private readonly UserService _userService;
        private readonly IEventLogger _logger;

        public LoginCommand(UserService userService, IEventLogger logger) 
        {
            _userService = userService;
            _logger = logger;
        }

        public CommandResult Execute(UserService userService, string[] args)
        {
            if (args.Length != 2)
            {
                return CommandResult.Failure("Invalid 'login' format. Expected: login <username>");
            }

            string username = args[1];

            var result = _userService.Login(username);

            if (result.IsSuccess)
            {
                _logger.Log("user_logged_in", new { user_name = username }); 
            }

            return result;
        }
    }
}