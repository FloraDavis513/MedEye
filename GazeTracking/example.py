"""
Demonstration of the GazeTracking library.
Check the README.md for complete documentation.
"""
import time
import cv2
from gaze_tracking import GazeTracking

start_time = time.time()
gaze = GazeTracking()
webcam = cv2.VideoCapture(0)

end_time = 0
total_time = 0

while True:
    # We get a new frame from the webcam
    _, frame = webcam.read()

    # We send this frame to GazeTracking to analyze it
    gaze.refresh(frame)

    frame = gaze.annotated_frame()
    text = ""


    if gaze.is_right():
        text = "Looking right"
    elif gaze.is_left():
        text = "Looking left"
    elif gaze.is_center():
        text = "Looking center"
    elif gaze.is_blinking():
        text = "Blinking"

    cv2.putText(frame, text, (90, 60), cv2.FONT_HERSHEY_DUPLEX, 1.6, (147, 58, 31), 2)

    left_pupil = gaze.pupil_left_coords()
    right_pupil = gaze.pupil_right_coords()
    cv2.putText(frame, "Left pupil:  " + str(left_pupil), (90, 130), cv2.FONT_HERSHEY_DUPLEX, 0.9, (147, 58, 31), 1)
    cv2.putText(frame, "Right pupil: " + str(right_pupil), (90, 165), cv2.FONT_HERSHEY_DUPLEX, 0.9, (147, 58, 31), 1)

    # cv2.imshow("Demo", frame)

    if (gaze.is_right() or gaze.is_left() or gaze.is_center()):
        end_time = time.time()-start_time
        start_time = time.time()
    total_time += end_time
    end_time = 0

    if input() == 'x':
        break
print(total_time)
webcam.release()
cv2.destroyAllWindows()
