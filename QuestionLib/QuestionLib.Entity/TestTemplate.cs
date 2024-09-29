using System;

namespace QuestionLib.Entity;

public class TestTemplate
{
	public int TestTemplateID { get; set; }

	public string CID { get; set; }

	public string TestTemplateName { get; set; }

	public string CreatedBy { get; set; }

	public DateTime CreatedDate { get; set; }

	public int DistinctWithLastTest { get; set; }

	public int Duration { get; set; }

	public string Note { get; set; }
}
