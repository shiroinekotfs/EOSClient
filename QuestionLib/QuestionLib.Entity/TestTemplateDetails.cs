namespace QuestionLib.Entity;

public class TestTemplateDetails
{
	public int TestTemplateDetailsID { get; set; }

	public int TestTemplateID { get; set; }

	public int ChapterId { get; set; }

	public QuestionType QuestionType { get; set; }

	public int NoQInTest { get; set; }
}
