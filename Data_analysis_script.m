clear;
close all;
clc;

%% ASF %%

% .......... import csv file ............
File = 'World Root-HostLog-Driver-2024_10_01_10_28_31.csv';
% ^^^^^^^^^^ import csv file ^^^^^^^^^^^^


folderPath = 'ExperimentLogs';
fullFilePath = fullfile(folderPath, File);

% Check if the filename is 'PutYourFileHere.csv'
if strcmp(File, 'PutYourFileHere.csv')
    disp('The file is still named "PutYourFileHere.csv". Please provide a valid CSV file.');
end

% Read the CSV file and convert it into a numeric matrix
SimData = readmatrix(fullFilePath);
SimData = SimData(2:end, :);
%SimData = WorldRootHostLogDriver1;
%SimData2 = table2array(WorldRootHostLogDriver20241001102831);

%% label Data %%
t                   = SimData(:, 1);        % Timestamp
x_pos               = SimData(:, 3);    
y_pos               = SimData(:, 4);    
z_pos               = SimData(:, 5);    
x_rot               = SimData(:, 6);    
y_rot               = SimData(:, 7);    
z_rot               = SimData(:, 8);    
x_speed_loc         = SimData(:, 12);    
y_speed_loc         = SimData(:, 13);    
z_speed_loc         = SimData(:, 14);    
x_speed_loc_smooth  = SimData(:, 15);    
y_speed_loc_smooth  = SimData(:, 16);    
z_speed_loc_smooth  = SimData(:, 17);     

x_speed             = SimData(:, 18);
y_speed             = SimData(:, 19);
z_speed             = SimData(:, 20);
x_speed_smooth      = SimData(:, 21);
y_speed_smooth      = SimData(:, 22);
z_speed_smooth      = SimData(:, 23);

x_rb_speed          = SimData(:, 24);
y_rb_speed          = SimData(:, 25);
z_rb_speed          = SimData(:, 26);
x_rb_speed_loc      = SimData(:, 27);
y_rb_speed_loc      = SimData(:, 28);
z_rb_speed_loc      = SimData(:, 29);


%% Plot Data %%
plot(t,x_rb_speed);
hold on;
plot(t,x_speed_smooth);
legend('rb speed','speed');