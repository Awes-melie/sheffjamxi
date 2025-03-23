using Godot;
using System;
using System.ComponentModel;

public partial class Submission : Area2D
{

    private Document _submittedDocument;

    public void _on_submission_entered(Node2D body) {
        if (body is Document document) 
        {
            document.ConstantForce = new Vector2(0,0);
            _submittedDocument = document;
            var response = DocumentEvaluator.EvaluateDocument(document);
            switch (response.Success) {
                case ValidationResult.FAIL:

                    giveFeedback(response.Issue);
                    returnForm();
                    break;

                case ValidationResult.MISTAKE:

                    giveFeedback(response.Issue);
                    returnForm();
                    break;

                case ValidationResult.WIN:
                    giveFeedback("The waiting list is 25 years long, so good luck!");
                    break;
            } 
            
        }
    }


    public void giveFeedback( String issue) {
        DialogHandler.Instance.ShowDialog($"This looks almost perfect... \nThere is only one thing... \n{issue}");
    }

    public void returnForm() {
       _submittedDocument.ConstantForce = (new Vector2(0,50000));
    }



    public void _on_body_entered(Node2D body) {
        if (body is Document document) {
            document.ConstantForce = (new Vector2(0,-50000));
        }
    }

    public void _on_body_exited(Node2D body) {
        if (body is Document document) {
            document.ConstantForce = (new Vector2(0,0));
            
        }
    }
}
