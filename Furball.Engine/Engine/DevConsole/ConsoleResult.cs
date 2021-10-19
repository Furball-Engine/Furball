namespace Furball.Engine.Engine.DevConsole {
    public class ConsoleResult {
        public ExecutionResult Result;
        public string          Message;

        public ConsoleResult(ExecutionResult result, string message) {
            this.Message = message;
            this.Result  = result;
        }

        public ExecutionResult GetResult(out string result) {
            result = this.Message;

            return this.Result;
        }
    }
}
