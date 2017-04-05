load Position.txt;
load Times.txt;

%Position = Position./1000;

x = linspace(0, length(Position), length(Position));
a = 1;
b = [1/4 1/4 1/4 1/4];

filteredPosition = filter(b,a,Position);

peaks = findpeaks(Position, x, 'MinPeakDistance', 15);

subplot(1,2,1)
plot(Times, Position)
subplot(1,2,2)
plot(Times,filteredPosition)

findminima = -1 .* Position;
mins = findpeaks(findminima, x, 'MinPeakDistance', 15);

mins = mins .* (-1);

filteredpeaks = findpeaks(filteredPosition, x, 'MinPeakDistance', 15);

filteredfindminima = -1 .* filteredPosition;
filteredmins = findpeaks(filteredfindminima, x, 'MinPeakDistance', 15);

filteredmins = -1 .* filteredmins;  

medel1 = mean(peaks)
medel2 = mean(mins)

medel3 = mean(filteredpeaks)
medel4 = mean(filteredmins)

p = max(filteredpeaks);
q = min(filteredmins);

sl = (medel3 - medel4)/1000;  %ta fram stegl�ngd i meter.

k = length(filteredpeaks)*sl; %totala str�ckan sprungit/g�tt

ms = k /((Times(length(Times)) - Times(1))/1000); % meter per sekund

kmh = ms * 3.6 % hastighet i km/h vilket b�r motsvara hastigheten given av l�pbandet
