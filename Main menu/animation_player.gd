extends AnimationPlayer


func _on_animation_finished(anim_name: StringName) -> void:
	if anim_name == "introText":
		var oldScene = get_tree().current_scene
		var simultaneous_scene = preload("res://MainScene.tscn").instantiate()
		get_tree().root.add_child(simultaneous_scene)
		oldScene.queue_free()
