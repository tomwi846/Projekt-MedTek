load Position.txt;
load Times.txt;

%Position = Position./1000;

x = linspace(0, length(Position), length(Position));
a = 1;
b = [1/4 1/4 1/4 1/4];

filteredPosition = filter(b,a,Position);

peaks = findpeaks(Position, x, 'MinPeakDistance', 5);

subplot(1,2,1)
plot(Times, Position)
subplot(1,2,2)
plot(Times,filteredPosition)

findminima = -1 .* Position;
mins = findpeaks(findminima, x, 'MinPeakDistance', 10);

mins = mins .* (-1);

filteredpeaks = findpeaks(filteredPosition, x, 'MinPeakDistance', 5);

filteredfindminima = -1 .* filteredPosition;
filteredmins = findpeaks(filteredfindminima, x, 'MinPeakDistance', 10);

filteredmins = -1 .* filteredmins;  

medel1 = mean(peaks);
medel2 = mean(mins);

medel3 = mean(filteredpeaks);
medel4 = mean(filteredmins);

p = max(filteredpeaks);
q = min(filteredmins);

sl = (p - q)/1000;  %ta fram steglängd i meter.
sl2=(max(peaks)-min(mins))/1000;

k = 2*length(filteredpeaks)*sl; %totala sträckan sprungit/gått
k2 = 2*length(filteredpeaks)*sl2;

ms = k /((Times(length(Times)) - Times(1))/1000); % meter per sekund
ms2 = k2 /((Times(length(Times)) - Times(1))/1000);

kmh = ms * 3.6 % hastighet i km/h vilket bör motsvara hastigheten given av löpbandet
kmh2 = ms2 * 3.6 