# Environment Setup
1. clone the project
  ``` bash
   $ git clone https://github.com/yxuan0329/FootCmd.git 
   ```
2. Setup the ``dataReadPath`` and ``dataWritePath`` in ``Main.cs``.
   - dataReadPath: The file path for reading commands. Download [here](https://drive.google.com/drive/folders/1SjdnZ_SQujV934D3puRcmUeQoZlC5MSC?usp=sharing) for StudyTask folder to read the assigned commands.
   - dataWritePath: The file path for recording study results. Set this path out of unity project is suggested.
3. Unity version: 2019.4.38f1, no Oculus environment yet.

# Unity Scenes
1. Study1: The scene used in user study
2. DemowithChart: Demo command input with charts
3. Mobile [unused]

# Start the Code
1. Select the scene in folder "Scene"
2. Connect the development board on the shoes with the compututer.
3. In Unity Hierarchy, go to Main function > ``Left Foot Data``, ``right Foot Data``, check the ``COM number``.
   (If the connection lost, keyboard mode will be activated, press number 7,1 for left foot and number 9,3 for right foot.)
5. Play the scene.

   
