#pragma once
#include <deque>

std::deque<bool> EncodeMessage(std::string message);
std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors, int& posOfError);

std::deque<bool> MakeErrors(std::deque<bool> bits, int& lastPosOfMadeError);

std::deque<bool> BytesToBits(std::string bytes);
std::string BitsToBytes(std::deque<bool> bits);

std::ostream& ShowDequeBools(std::ostream& os, const std::deque<bool>& bits);
