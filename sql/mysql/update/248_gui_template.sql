-- update the help gui text.
UPDATE `gui_template` SET `text`='^7A^3leajecta ^7is powered by Drive_LFSS 0.4 Alpha
^7
^7This is a alpha stage of the developement
^7there is a lot more to come, please be patien.
^7
^7List Of Commands:
^2!help^7, ^2!menu^7
^5!say^7, ^5!manager^7, ^5!exit^7, ^5!reload^7,^5!loadrace
^5!endrace^7,^5!searchrace^7,^5!yellowtime
^7
^7Did you know "SHIFT-i" will reset button and make Menu appear.
^7
^7 Your Host A^3leajecta^7.'
WHERE `entry`=3;

-- menu
UPDATE `gui_template` SET `button_entry` ='133 146 147 148 149 136 137 138 139 140 141 142 143 144 145 150 151 152 153 154 155 156 157 158 160 160 160 161 161 162 162 163 164 165'
WHERE `entry`=18;

-- config
UPDATE `gui_template` SET `button_entry` ='22 25 26 27 28 29 37 38 40 41 42 43 77 160 160 160 161 161 162 162 163 164 165'
WHERE `entry`=2;

-- text
UPDATE `gui_template` SET `button_entry` ='45 46 160 160 160 161 161 162 162 163 164 165'
WHERE `entry`=4;

-- rank
UPDATE `gui_template` SET `button_entry` ='47 48 57 58 59 60 61 64 159 159 159 159 160 160 160 161 161 162 162 163 164 165'
WHERE `entry`=5;

-- rank
UPDATE `gui_template` SET `button_entry` ='73 160 160 160 161 161 162 162 163 164 165'
WHERE `entry`=6;


-- NodePosition
DELETE FROM `gui_template` WHERE `entry`IN(19);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`button_entry_ext`,`text_button_entry`,`text`) VALUES 
(19, 'node position', '166 166 166 167 167 167 167 168','', 0, '');

-- MyStats
DELETE FROM `gui_template` WHERE `entry`IN(20);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`button_entry_ext`,`text_button_entry`,`text`) VALUES 
(20, 'my stats', '160 160 160 161 161 162 162 163 164 165 172 173 174 175 176 177 178 179 180 182 183 184 185 186 187 188 189 190 191 192 193 194 195 196 197 198 199 200 201 202 203 204 205 206 208 209 210 211 212 213 214 215 216 217 218 219 220 221 222 223 224 225 226 227 228 229 230 231 232 233 234 235 236 237 238 239 240 241 242 243 244 245 246 247 248 249 250 251 252 253 254 255 256 257 258 259 260 261 262 263 264 265 266','', 0, '');

-- add green flag
--DELETE FROM `gui_template` WHERE `entry`IN(19);
--INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`button_entry_ext`,`text_button_entry`,`text`) VALUES 
--(7, 'green flag', '79 80 81 82 83 84 85','', 0, '');
