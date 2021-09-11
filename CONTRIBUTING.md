# Contributing Guidelines
This page will outline the guidelines for contributing properly to the project<br>

<h2>Committing Guidelines:</h2>
 - Commits must be buildable<br>
 - Changes in a single commit must be relavent, so no putting random changes in the audio code while naming the commit "Add new input method" <br>
 - Commits must follow the formatting of the project, so make sure to run a formatter using the EditorConfig before committing<br>
 - Commit names should always follow the naming guidelines<br>
 
 <h2>Commit Naming Guidelines</h2>
 All commits should follow the basic structure of "SystemChanged: WhatChanged", so an example would be "Audio: Fix pitch check", if the commit changes multiple systems, then you would write it out as "Audio + FurballTestGame: Add frequency changing test"<br>
 <h3>Note on merge commits: Please do not name merge commits "merge x into y", please explain <i>why</i> you are merging x into y</h3>
 
 
 <h2>Pull Requests</h2>
 If code is to be merged back upstream, all CI checks should pass and all guidelines should be met, with exceptions being given in cases where matching the guidelines would not be possible
