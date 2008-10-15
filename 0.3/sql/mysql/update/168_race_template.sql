-- Add  new Race Template
DELETE FROM `race_template` WHERE `entry`IN(21,22,23,24,25,26,27,28,29,30);
insert into `race_template` values 
(21, 'Fernbay Green Rev, FOX', 21, '18', 0, 0, 7, 0, 0, 1),
(22, 'Fernbay Gold Rev, FOX', 23, '18', 0, 0, 7, 0, 0, 1),
(23, 'Aston Club Rev, FOX', 45, '18', 0, 0, 7, 0, 0, 1),
(24, 'Southcity Town, FXO/XRT', 14, '7 8', 0, 0, 7, 0, 0, 1),
(25, 'Southcity Classic, FXO/XRT', 6, '7 8', 0, 0, 7, 0, 0, 1),
(26, 'Southcity Sprint2 Rev, FXO/XRT', 11, '7 8', 0, 0, 14, 0, 0, 1),
(27, 'Southcity Chicane Rev, UFR/XFR', 17, '11 12', 0, 0, 7, 0, 0, 1),
(28, 'Southcity Town Rev, UFR/XFR', 15, '11 12', 0, 0, 7, 0, 0, 1),
(29, 'Fernbay Green Rev, FOX', 21, '18', 0, 0, 7, 0, 0, 1),
(30, 'Blackwood Rally, RB4', 3, '6', 0, 0, 9, 0, 0, 1);

-- Add  new Race Template
DELETE FROM `race_template` WHERE `entry`IN(31,32,33,34,35,36,37,38,39);
insert into `race_template` values 
(31, 'Southcity Sprint2 Rev, XFG/XRG', 11, '2 3', 0, 0, 10, 0, 0, 1),
(32, 'Fernbay Club, XFG/XRG', 18, '2 3', 0, 0, 12, 0, 0, 1),
(33, 'Fernbay Club Rev, XFG/XRG', 19, '2 3', 0, 0, 12, 0, 0, 1),
(34, 'Fernbay Rally, XFG/XRG', 28, '2 3', 0, 0, 9, 0, 0, 1),
(35, 'Southcity Classic Rev, XFG/XRG', 7, '2 3', 0, 0, 8, 0, 0, 1),
(36, 'Southcity Classic, FBM', 6, '17', 0, 0, 8, 0, 0, 1),
(37, 'Aston Cadet Rev, FBM', 43, '17', 0, 0, 8, 0, 0, 1),
(38, 'Southcity Sprint1, FBM', 9, '17', 0, 0, 8, 0, 0, 1),
(39, 'Blackwood GP, FBM', 1, '17', 0, 0, 7, 0, 0, 1);
