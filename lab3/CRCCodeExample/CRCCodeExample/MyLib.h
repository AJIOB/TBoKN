#pragma once
#include <string>

std::string EncodeMessage(std::string message);
std::string DecodeMessage(std::string message);

std::string MakeErrors(std::string message, int& lastPosOfMadeError);
// returns true, if error were fixed or if fixing non require
bool FindAndRemoveErrorsIfCan(std::string& message);

std::string Divide(std::string dividiend, std::string divisior);

void CheckMessageLength(const std::string& message, const int requiredLength);
void CheckMessage(const std::string& message, const int requiredLength);
