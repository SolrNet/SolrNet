A collection of MSBuild utilities. 

Currently includes the following MSBuild tasks:

For starting and stopping processes using Process ID's (pid's)
 * SpawnProcess with a Process (required), Arguments and WorkingDirectory parameters. And a Pid output paramter.
 * KillProcess with a Pid (required) parameter.
 
Import the tasks by adding the following imediatly after <Project> element in your MSBuild script.

<!-- 
	This will look for the file 'Endforward MSBuild.dll' in the directory containing the MSBuild script. 
	Leaving this out will make it look at $(MSBuildExtensionsPath)\Endforward MSBuild .
--> 
<PropertyGroup>
	<EndforwardMSBuildPath>$(MSBuildProjectDirectory)</EndforwardMSBuildPath> 
</PropertyGroup>

<Import Project="$(EndforwardMSBuildPath)\Endforward.MSBuild.Targets"/>

A complete sample would be 

<Project DefaultTargets="Compile"
    xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <EndforwardMSBuildPath>$(MSBuildProjectDirectory)</EndforwardMSBuildPath>
  </PropertyGroup>
  <Import Project="$(EndforwardMSBuildPath)\Endforward.MSBuild.Targets"/>
  <Target Name="SpawnAndKill">
    <SpawnProcess Process="ping" Arguments="google.com">
      <Output TaskParameter="pid" ItemName="pid" />
    </SpawnProcess>
    <KillProcess PID="@(pid)" />
  </Target>
</Project>

For more information see: http://hg.endforward.com/endforward-msbuild/