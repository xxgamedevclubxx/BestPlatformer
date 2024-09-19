extends Node2D

func CheckRunning():
	print("I am running for my life")

func GetDeg2Rad(x: float) -> float:
	var y: float = deg_to_rad(x)
	return y
	
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
