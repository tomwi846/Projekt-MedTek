load Kneeup.txt;
load Heelkick.txt;

x1 = linspace(0, length(Kneeup), length(Kneeup));
x2 = linspace(0, length(Heelkick), length(Heelkick));


%filteredKneeup = filter(b,a,Kneeup);

%peaks = findpeaks(Kneeup, x1, 'MinPeakDistance', 15);

subplot(1,2,1)
plot(x1, Kneeup)
subplot(1,2,2)
plot(x2,Heelkick)

% findminima = -1 .* Position;
% mins = findpeaks(findminima, x, 'MinPeakDistance', 15);
% 
% mins = mins .* (-1);
% 
% filteredpeaks = findpeaks(filteredKneeup, x1, 'MinPeakDistance', 15);
% 
% filteredfindminima = -1 .* filteredKneeup;
% filteredmins = findpeaks(filteredfindminima, x1, 'MinPeakDistance', 15);
% 
% filteredmins = -1 .* filteredmins;  
