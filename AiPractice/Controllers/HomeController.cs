using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly OpenAiService _openAiService;
    private readonly HtmlParserService _parserService;

    public HomeController(OpenAiService openAiService, HtmlParserService parserService)
    {
        _openAiService = openAiService;
        _parserService = parserService;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(string inputTextOrUrl)
    {
        var model = new InterviewModel { InputTextOrUrl = inputTextOrUrl };
        model.ParsedDescription = inputTextOrUrl.StartsWith("http")
            ? await _parserService.ExtractTextFromUrlAsync(inputTextOrUrl)
            : inputTextOrUrl;
        var questionPrompt = $"Generate meaningful and relevant interview questions based on the following job description:\n\n{model.ParsedDescription}\n\nGroup the questions into categories: technical, behavioral, clarifying. For each question, provide an example of a good answer.";
        var scriptPrompt = $"Create a detailed technical interview script based on this job description:\n\n{model.ParsedDescription}\n\nDivide the script into stages: warm-up, main technical block, behavioral questions, candidate's questions.";
        
        model.QuestionsByCategory = await _openAiService.GenerateInterviewOutput(questionPrompt);
        model.InterviewScript = await _openAiService.GenerateInterviewOutput(scriptPrompt);

        return View("Result", model);
    }
}