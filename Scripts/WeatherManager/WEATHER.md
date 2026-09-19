# WeatherManager

Design: `design notes/18-weather-ambiance.md`

Weather is its own system (field + bake + director). It does not live inside StatusComponent.
The manager samples the baked map and hands tag *names* to Status. Status applies them. Weather does not Hurt() and does not own HP.

Wind is cooked at level build (`WindPropagationBaker` -> `WindPropagationMap`). Runtime is a lookup. Live raycast is opt-in when geo changes.
