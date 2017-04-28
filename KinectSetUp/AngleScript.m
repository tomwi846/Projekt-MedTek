load Kneeup.txt;
load Heelkick.txt;

x1 = linspace(0, length(Kneeup), length(Kneeup));
x2 = linspace(0, length(Heelkick), length(Heelkick));

Kneeup = -1.*Kneeup;
Heelkick = -1.*Heelkick;

%filteredKneeup = filter(b,a,Kneeup);

Knee_peaks = findpeaks(Kneeup, x1, 'MinPeakDistance', 15);

Heel_peaks = findpeaks(Heelkick, x2, 'MinPeakDistance', 15);

subplot(1,2,1)
plot(x1, Kneeup)
subplot(1,2,2)
plot(x2,Heelkick)

Knee = -1*mean(Knee_peaks)
Heel = -1*mean(Heel_peaks)

Kneemin = -1*min(Knee_peaks)
Heelmin = -1*min(Heel_peaks)

Kneemax = -1*max(Knee_peaks)
Heelmax = -1*max(Heel_peaks)

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
