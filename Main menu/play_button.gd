extends TextureButton

@onready var anim = get_node("anim")
		
func _pressed() -> void:
	anim.play("introText")
