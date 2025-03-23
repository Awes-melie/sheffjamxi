extends BoxContainer


@onready var text = get_node("dialogText")
@onready var anim = get_node("AnimationPlayer")

func _ready() -> void:
	ShowDialog("woof woof woof woof")
		
func ShowDialog(string):

	var text = get_node("dialogText")
	var anim = get_node("AnimationPlayer")
	text = string
	anim.queue("play_text")
	
