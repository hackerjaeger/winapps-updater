﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/121.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "fc450e2a8ac34bc5f4a388d104a978aa71bb82fc5007d536085b6d8dab3f8ec82dd9597cc2a467b4a2203bec65765ffd8b8d9bbe3be87a1144b1520fcabc9537" },
                { "af", "d0e18cbc887de14d32099a506def096100af0b352256b7d3f121b42ccd45823e8f5232b8003d66bfa6a777893218c4a790bf18880dbbccb810f86b724d126462" },
                { "an", "a83d2f59592ce439fa6d88e91c8d4e191a337132b0c46eea9fc78057d19e385df98c0b8ed10d51158aeb88cd1ec3c2dff0b98022b3c1a7e32f7f30144ac9b09e" },
                { "ar", "a66f82571b237c990432ccba97ac49438f5da25d8273656052209d78f44b121d2e208dbfafc2e5c2103658e598c65eb77193f68eb4c451adeeba1527e45468dc" },
                { "ast", "4b2953d093a6fdad14fce53abf52a828e53d88a7d4d05a71639412a207840823911a34db0d431d6766315219aebcc4a3e2004fa8714ec73d4845a52887159df1" },
                { "az", "0586ecb478b60faa4f64b4f42cd2585afc0e60ad3bf4c4550eadeb40bc9f619b184762cc97db7fc648ad7abd28e175a877a4fb7efa60b29865e50515d05f539f" },
                { "be", "7e4289735a60b39bfd8361619c38323cdfcab08cc5293f75e8495ef040e37118353c544df721a640436f10c89c83abb843a729dae19467d5fe242f0f0b0a2976" },
                { "bg", "513ba4e34fe5078808d65c5963f087178005241e41316c50abc2313e954b5e3dd2e7609343d031ea6aee2306718b8dbd10b4b70ba72708bef275926db826f589" },
                { "bn", "0f81edf82c26903066f93950b44456a15ea7d4913f47c07557959ce6cc20a5362266966812e2da2df9b87754bf4f37c92cf8294e712d8d3a830119ccc9fe9bcb" },
                { "br", "14738129eaf9e7f024caf0275243454083aadf64a6e98c359749d0d2c1e54877a53c22f7f9229fe381814a0a14bfb70abbe17a07a275986c0605e7b6daace060" },
                { "bs", "6cb48f6952c345d51c19597c72ab6884fc09ca4de79e2ce25ea271a8bbd80ca96c30c19e59260685f1d6ccc794502ea29d1d2f300335d96d013ca94eada3e65d" },
                { "ca", "4855335cd551ee5d9e26f537a8df33f6a9e23f152fc83cc61d5c16e0831fba1e4153f2ec8e49896e5ee7f8757fc24d9b676352a57aa4a29ae3397c15c0c80c54" },
                { "cak", "0ea658320733131cf095224f99b9459d9ac8d6ad59f8a2666e01d9e3c9b3f5831acc31e9907642f273cc448189ea13448d06e7d7a11c100a6f19563d299ccd60" },
                { "cs", "207c1453f0e9c424ec6a4afbbbfbda255b417bf9276a13fe77d2624a9ae2811335511ecd291bf68ef9c7e60c82e87df523ed5ab459bb19d8c4b74e53734732aa" },
                { "cy", "7a2fe1e92d57ccad5d5fd2cf423a3c7ff220e200147a8519fb4e9940c59d59ec48e952719f0306b8d91fe0c8c9384fbea6955c563a7c31d0246984604aa9305a" },
                { "da", "c1aa0eb3578de5d87bcd2a4fcd8c6ce8b8d4aa857da498d56bb42288f937aba485d44d5f61996f5ca93ef5111380c9a9a24535bdba2052c9741b63d77157a347" },
                { "de", "e4bc39743f3082440157d50279ae5b6f6d2201e4e9e15a126316e960b4de4ab4eb71c51aa8300e9349a1965153e6ca0f2b20e6e5dea666a9337cd6dd31f0cd71" },
                { "dsb", "2155e737fa62bf5bef0ed45a1267a923da5f5a53fcbcec72ab96aeb6e5d7aace517c4273e9806b0d6016fd9d06998510864fb598e86a54a327015b033782e4d5" },
                { "el", "396bc05c5f7713a67aed4f842738721722a50c9d5a4daf06c9c42d9fa5cb85758c923b7d63abf786c285ccfdad838517a6412dfa23d40e298a4a381c3a95ca46" },
                { "en-CA", "49f9153fbd3d5cae4a70a5b6206a076ca0809ccf42ee0d872acab17b682845fb311ed990b9db573328678fd0d8e5e801bbe8d9d3bc82508f960f8e696a256f0b" },
                { "en-GB", "3935361c8643b3723702f2e0f95ff40403c3f25f1d8580364f817d481e371111b5c8f7e332ab9c84e9976e9671cb799aa81b26ae30130a57732200ea3fb43d0f" },
                { "en-US", "55e41ecee972d0919f3d06a43edb8f9366f17bcc4f84da3a932873189a8fa90c0b28fd5f6d921f46837794a8b9f6444c2fadca8cea55f05ba271c57f4922831d" },
                { "eo", "3bb564fe2c298565f2aa2a93db8de2d910b7ca0076484f62a3323dc3a12b832e3bedbee04ead7a1a41455848f733e105745a13bdb3c7230dcb944c56de4d2c5d" },
                { "es-AR", "f2aada2921b0bf6383f32adc5c0879ae1c5e5e321d05ad3823922bac2f81abd7c8f54af251436bf1bf2de46311fbac67922116ae73b8b5007bc6b356918ae902" },
                { "es-CL", "41d39665d95816619d9cf85d7c1ec2c3fd06ea940268cfd8d0d0b794d6a0c6d5a1e6a8a43f77e1d14fa64abae69ec90fa17321f74bd13be9ed217ab8a122dd52" },
                { "es-ES", "756cb06193caeb55b787df7661c5e3477864f049b360bd8e6b93735079c61a0b69149e56459a878e488fc2fe3ee5ab1a9d1924310efffd4d6e5bb84d09048568" },
                { "es-MX", "bde0ed210739112ba7eb1e823fdda4d7dd4edc41baae7e912862dfb9062840dd59318d9396372c74717c4b7d9dd275478a590084dc4ee040892c7989285db10e" },
                { "et", "c95efc38c36fcf08aac52d8d752bb06250f06f143a301c32203667eafd1eed903d9c6d49cb6553ae445152da08aed91f4e300f1bd112a194be07d9d11911fb67" },
                { "eu", "546af0122ac22633ef47044cb932f52ea0484ff201d7c983f5ad03e9a300ef606faf12d530d53ad0f357925a4a3450398e155622bfaaf8c672db8caa2e0f0bdf" },
                { "fa", "353bff763269e268e8d0ee65b509c34d154ff1b5f2c51e58f21b3ef2b96c8fc9b2d535fb1a74e168988dceba9105ad6187e42420f394ad2d3ab577b46393d909" },
                { "ff", "09e0a8cede7cff80dae44e3baaeffee783e2d81ef5c80226e71c26a9a590085351b72ce4dedb6c5661e7ef1986272c7143afef7a949b0e80a643a2c689dd43d8" },
                { "fi", "7e5e30e6eb89c01a30b12a13bd9b42e866a5ab9f3ca94002c94594277e7172910665d1af448c1aa6563a372ddfcf55bb23685c8c5882a1cb17ed1ac8395d2035" },
                { "fr", "99b77540e3b51940484196fe924cab313f5379f8d4a1181e58b40c17a3332068385890c8f59773d526de4c8f0568b9f790182fd69977fff408ea13617499adef" },
                { "fur", "8c5ceccf15e446b175f6d054beb6d417ca26f733be91aaab582df54927c5cc1aae3f76c91d38b1f567b9de17807fbffc395a011fb7477ace670c6398ae7e82a4" },
                { "fy-NL", "f8dda08e5508ee244971d950e21718dec3112d4247ac4c13c045a01e101d4125896805c27d1bfdc069c4da865ed8dd9682f62065353f21f4af2aa2aa90350415" },
                { "ga-IE", "be6d19760489269e92f69a5284375068abc37b61c98a45d4427219f28c1178536d5eda3f0735f22895676234243a0cba66984366b04f9d21572cdfccd1b07c87" },
                { "gd", "92ba870613037aab4f270c73d1f88b46dcd98f1116113816c3ae3dbafeed866f99fe40c34ba28888c6981911a5f5c666aa8e9139d3d9fea6cb83018be7676565" },
                { "gl", "68d5cf2caf60a2bb9796f9cdaf62013609d9a84857e2925fc8ab7364e093bace5107891ec0c01338e8530f469fa214919c166aef40f9e7d824a55865cf4164b1" },
                { "gn", "ff0a14927b867c51bd7fe12a2b50795173f7d8c5f3308ac8a1c5e95a8bea6421e5c5f718f057518e47965009352b80258991eb9d77f24d9a127f5283d2a5dffd" },
                { "gu-IN", "da96c27369830ff7891d5c71e67b0e0c35174dfbd57bf50bbdc14a3afba7b6311f9800b5d56def4061177b829d1a03c56dcc448a285d12c29dafc7ff4f11c26a" },
                { "he", "91103883ec1b7357cf59116ec8d112a17ae67ffae3a695ebb9d23439be7407fd5171c06a744eadbaef417f7df43b7a9e1796cf434b0d871502c4d1000d13285f" },
                { "hi-IN", "3c14d156f62d11ef522a87bcd26f7b92c84a7ee8790488b00838dbec17aefae68e74335b1f1933c7e59b391315494dfbb590b773b28f320a5bc343633152f3bf" },
                { "hr", "6ba661a28119adc384792a6684a9a866ddfd15409fd655ab42b881fc82dc6c6e957171f23df0382ae2e06948aff3e2c3a34d78bbd3d152a21605c8abb907dd17" },
                { "hsb", "6423df0a9f9736be6f5ceab32d6c2c374298ea96fd34943aa914da1da0dc503ca5e69c03c4ad73f93022ea48dff405be256b5447f47bcfdb69674b7b0614ec8d" },
                { "hu", "925a9594bd7ed2a3a055c75b5e54b0255fd7c196fb30343121bd1624cfffb27cbd5ba6b440128332ef9944f12befeb86701ce271f313072c4a776dd7e57436ef" },
                { "hy-AM", "6a903999f125063c16e65e609cd9f2ab9fbd8ade91e9d970677f26e00606c3dd7b3f7b529f0c17aa6744e94ef80f3b21ce03d6fb838a4cd572478f6abb61e1ba" },
                { "ia", "00b40247c1636073ffe18b3c888bf29d47e3d6f2d7ba46acfcfec20c4066625d6ec861218958b5bc1a48adaceee8ec0be73820a1a4628252d2f5a76c1b7f39ed" },
                { "id", "41b7814469483dc6cf9df0c01dc0a99dcceca6cbce9b5bdf8a85a2521d0073adf3b21a6236aa4627b420ab5447fccf7dafa5d62e4517a2c3cde6354cde0c0cd3" },
                { "is", "e64f08558f3081f6e1baf6e6cead7e58e3db41410974a1017c705c646fb6b7210a8e9c9cfd86f888f925a4b943689f6381141840ec9d1ead65ce75c7405affc6" },
                { "it", "0be484f843bb87b1192887a6584fdc174edb9808ec510183a3ea5b9a4a063375fbd290a31bf99d21d748d50a7efd48f99f0cd4257d56293e51f2ab65d6e11f28" },
                { "ja", "649601bee260839e11d7afd195c90440e61e2140335b6e8d689d3ee9ebb1d14894e106ddd22d9470651b5badea9b0ef52b1d8b79038ce4091942f0ced998eb52" },
                { "ka", "f287c739429a30f83ccd7c4f7a50bafa5a90e0cb1c60fbc58aabed2f303cdd69c53e3bd397be446f7b2e5cb35598359b755b4c3c282afc6be2707d0deca3bbff" },
                { "kab", "ecc0df8c4a1ca5e869d13a67f8c7adbe453ff3fa59d5ac954fd7a687f1adfe36924993388f8cf71ad50e8797deaecdc08a3252706463aebb61b5f6d5e884b541" },
                { "kk", "0314a8a5cd1368eb397ca20d6ba431cf540b2d1ceb6626140003f8ab4ce7da5c748dff313063ef8410fb0820d663cebf8dcce1165ed05ceb6e2fa7af9ef67632" },
                { "km", "6c0b45c0888b71e11b17617e48cba917f269a31c0addddd0854036fce43d8019fd02bedf786efd80afbe228f990729f95960812b9570e2dbbc0fd96340005c7c" },
                { "kn", "55a827bc63b60d1748bbe27e020e929a0e6588a539bb0d5c58b52075e38d07a82c991f67f4f5fed2c17bf45d01495180328cc8dc7b91ee65646e1d14ff9a33c6" },
                { "ko", "f843cf23e136f3d585f61f48748116c5149c1f1359eb60ad29225c22ce99ad24085bd5bb0f79825634828b5fd23f1e1124d7f634f01f0182d3118400878b1c0b" },
                { "lij", "204bc86f2484db7ede505676d359d35b2586b4932c17b012d771e1b2e681d759d1eb4364dc3307800cdee7ba34125369ea6e11057a8388c675dc80dd6f4d2a5c" },
                { "lt", "efcb7a4fbd016e0c376b54fad442bea9f1d66292765a421b9ba9d2e042aef29b8d8b25ae8c68081938b59ee5bf6df8766f199f28bff459fb748b2b6b21f0e068" },
                { "lv", "08a22196a632bcd977beea7a8cfa2ef4fde7c644ad431ce688030d18605d0f5ff354ed8428b39fd0a5822dbf2d5862871982183068c1546887d93a78129ad349" },
                { "mk", "37f69bafbde7a309ebce42bd0df92ac1387ffac6a4f0f309ba86589c0d5e5a5f27306c8d3caa365b14e63e23259ea10ea5e973917e6d83a8b58179c63403e088" },
                { "mr", "6e62b549e5def7b1aabf56afc9f9c830ba29117c4023e5a0981772c592ee474dc50c4479794405b6f9ae06b7a983bf5821a9c43425dce5a6429bdd941f9995be" },
                { "ms", "a75cc6a2eba19992b293424090be5fc6aa1414fdab9ee1037f4c4501effcf35ea679e7e69862dcb6feed3eea6bce607a8994dfb82851f0446ded7153b886b482" },
                { "my", "e68c743be02e53c6a1fca8dd60b793649a14315d313247cef2f9f293631bcd28e502ce2a975ff4188ee154ef38895cfaf094d09f5512e091a14a43d1a543e714" },
                { "nb-NO", "da296577b072425fb0ec2194a855934fd9858eb24b52c50e20134de7ae8195197b9a56b314f0411bbf0c3b446940bc79d1f121f27c1ffadb86366ff01113cb39" },
                { "ne-NP", "aafe412ae6568bcabee2adb8807dd4408d34663201e567da6bee72d9fd1317aac734be9dc9d87dbc1f0dae0c056b74931878298011a9dae3c6b38c8efbea642b" },
                { "nl", "9c1d154a492e48ba27c2c52b0cb7721c2fad3f2426c830626690ddc70a8460fc2c7812acd85e6533b1f6fc7699951ab01e887373e360133326e9cbe5dc2b2869" },
                { "nn-NO", "2f72dc87a09937b4dc9b8f3008724f22b6d1ee0e41ddda8c9d0309a93a44a7ea91bb43e688f64b9eebf784472949ea7cbc9438aceda92f6c104d17ea5c0c3541" },
                { "oc", "8acdb7c9af93af14f6d28a7382a6fcf99d5f9c9edc01f970e8a4a02980760d0d5ed9b00e29b3ee164cd6ba2448225d6f3e4e5bbba639057b6507d8920d013f11" },
                { "pa-IN", "4436295d96e939a0f08799228e59bfc2c33e3d2a04a81576d70ca4481a26eeeed6a9739dcad12e8cc27d5886e15bbd737797dcfb4661c15deb28c2b44b98cd24" },
                { "pl", "c6a5f887434f1195653edb9c2622faafe8ed6f8254a73566423aef17920ea58a72acdfa6a13860f6a5d4bd4395e1b446dbe882baf9469e5903df0cf2afc0f027" },
                { "pt-BR", "c67fcf80fb1da8480941cec55a5585f201d9fd9b7def5043445d4340288fafd3e2affd0627634d6b9fb01c94dbca9148cf604be84b053ff7bde619610537a8ba" },
                { "pt-PT", "db76a9344b9959c458af9d37f24f01e6b683a0dca9647e9fc0318b8de144fc1555f83b305931ee9082d29195c99b2faedf5ac982bc1d118cefe4fc4e8090d81f" },
                { "rm", "5c10045dc27cc52c43c40d972695f957b9864d4b3ff22cf9c164caaeb0e06e9c7dfdc2443161e0692f5676dfb7cfc54dc2ea4747e763883389aaf0033a97e97b" },
                { "ro", "b211b212496df5911edc656accf1bf80f688c729ecd18b38517728cb12dfa53c55573e9d09d773d90a92ffa0ecef7268093c6dcf921d5c8e4bc1aadad859bc60" },
                { "ru", "691a84951101efd239b4c81a2a9d33226a8d8f54d3b551719f00e04dc4e7e4b821270eeda9f6ba5dd5fdde2b3daed4aa0cf0349071f26029eb2d812723f22c80" },
                { "sat", "e4f9dfc47449fe02c4abaff3794d4d5d01fdd3a7d5c00ae46c53f79b6b99e4b1eaa2045e1a07539bec466453b9e41bf0bae5506415904cd6f49f5ddc307c6a1a" },
                { "sc", "c0dfac721251f0a5cf180643bcb7a66e6929f91540079a09166f9bda05c2a0d7254144a5128d659187064ec0e7363034f24b80b3bd706e852f0951599ee85cfc" },
                { "sco", "ca863056ee592854cf92d6eee2c55a9b0f4bc2336318a077e222013f3eb9375ee600bb3ccad1772184c4768c33776ab516c35c25d7b132e56e2d0f0aab5242af" },
                { "si", "cbff4b8307e8fb707a9e33204bd52329bb3210e7825f0c602a688fd073734ef5b767c4e1a536a96fdfc25c45106479efc53bddf23b6fe29e42797b7ae222883a" },
                { "sk", "fefbd0b36faf8cd73e65dde7699ba93869756eb5bbc11d7126689019faa6fa5db2f9a9e17268e2001b1117e1beb7c7964d8c22a77020302cd28eb7b365c8f072" },
                { "sl", "0e017889bcb92d211d998432e288097d0e621bc39b5f209c0c655a0be90ee8a6e97d006140229c22bd43c66a6b165fc1b5e2e4840977fcf2eacf3d0684e875a3" },
                { "son", "752e1e3990c367636522defabc26c351fc2d885d3435aa1e6165bf0cad0a15a780f5450c96c0579c6497ea7048b419b4a11e5ffd921b922727d68efc05b3bf3b" },
                { "sq", "003902c04bdff17212a45abd142a241e7f70b9829e00dc6be2bba16d7f590a9b062830c103eee40407ce1cc5323ca7bc68f4824b75e1fb5029976197ac8e3d42" },
                { "sr", "c4800b21a7e3d6b6d93c55283a18e9f4307ac79f00159972e1200a59eb0a2b7f80c7ff58f13e8a8bfb980aac4a1a17cee2a844064930840a1ca3cb89194b87d7" },
                { "sv-SE", "218b7a1c055200c11e9531f25b4d451b1177245b09a9b3e4839a7167379a62d607826bf12066d85a1e0e8a8ffbf4c46f3eeaa6eb60b002535e73adc6e570a2cc" },
                { "szl", "6fcfed378658698d72991591315ef956c1e1a778c071ecfbccef758c1e5ad453b35b5eedb909c801d599af359ffae830669dc2a6709dc65bd72a073b00449cad" },
                { "ta", "f7c50ec4066d194f48fba07f046c9eb5ffef9feb22873b9634601fc0e73f03881e2dee37beb5c8441c0973109711082ba97d966f85715d34bc208aec63b11b11" },
                { "te", "fd57481bd483f2f639f16f3b6736d85cc8c442ede217c6b0bf35f5ba5eca49244028d4373bf11307df83f2cbc9cf76546b4f9c7a1fa6708c68c47a1c0a023d2e" },
                { "tg", "a0970d7f618221e70f2a89f5ec0836d48229384e82dac78fd926062f01f6856c73877b4cd0ac8c62d7a4431587b082620baec560e0a2c971024b19bf0e4d2998" },
                { "th", "102c64d2d3d0d7e3b261d5460f6db0e3401e826974cade892209a6ec333705e21fb264cd095ecd6fcea1c23507103689254b8f3c28eb5b539fa0ccb6f5374e01" },
                { "tl", "65a377ce7ac6c579adcfb12608b67f8dc0ac1be286425926a26dafda80caa9bebb1816a49c16f9edc94d6386dd9bf76774613d7c80b8fc601262fe9b6844575f" },
                { "tr", "43163d14a7b734a1b7245ab8b8bc96763be7d373d8d635a2ef8b4b4b67b68a4ed48ca30ef2f7d2b69638f052b0caec4df8f26247cdc98a367569917ff9aa7679" },
                { "trs", "cc01654393a8b40eab83bcbd7b7c1543852aa8a9dbc9c884ae08ce2f86bd7b7d36abc8fcea7f3b418cd440650dcc58a6416c17e8c733c64446f16d345a94fcc6" },
                { "uk", "d0603fc95702a11eea32d04d45ff4f701e93ce32ad0f6deb30193056cc011a30aecaf565fbb077affa3baf1fd337d4275e85e0027de70ceea897dd069cf8b45d" },
                { "ur", "eb3060ae74104722377254c2447e695aec53104ae37c302c2e0a1fab64c885b1da76e1e9e098dbc86fef993de9b76733327a835e313528768d789f99fe31952f" },
                { "uz", "3d955c8e9d4de24dc8e7e92144a6eaf73fd2ead7c4929a0326d4247b2ae6a9e4fbdd694371dafb2de253cb09deb14de0d4564476239af545a76ad1781a5292f3" },
                { "vi", "3c95f0554c16d0b921bca8a05df185fcfce47b49d4bc301fc419c3339494e989ada094ae5852ebdc9e8fe05fab8d2dd41835fd5ed36248737ab8beea6fc51c69" },
                { "xh", "f83c0163a9daeaac8e59e87d5e2493318c0a03254a4b29c7945701725b2fa4e183b3c3ef71f857f183227b9a6f44e638f69a6914b84f7ffa25d3b012f1041546" },
                { "zh-CN", "00adba6e688bf8c66d1ceb1708318b35a1df6d5fa8be68da14400d6dac8f8dc1bc1dc3c6881304b6479cba39e27b35aacffff5124188836bc9fdf421a4eb59b7" },
                { "zh-TW", "e14f6eebcfe308fa78cf8d2ef7019d1deca5d83717b25d2ef19a8c18bd62da011fa84698bfade0c958ac71712a6e33aff787eb3530ace1e7c7822a59105f9b8e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/121.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "e1a6a216eef732e340583143937aebf987efcc85fc8202a5828392aa179a214c066dfcd8ee49db2414c1c625352e2d6da0906a5946c13b7a4cd99bcc4cfb8bd8" },
                { "af", "ac26a9af0d74caa73af9fd6c44a86f5cc0fa91b59665831fc1289a6bc415ea61c4eb0893f0b65de88567c8e0076e0bcc40b3ea49e38fa598502b2714e8c2517f" },
                { "an", "0be9a362945480ceec4c57bf928368fee916632612caf320a5a72c2f8b58c82c49d13dfd873306e328788f02b1aa8f08a96bb598a9a5a47b339b9ed68d812d30" },
                { "ar", "4a60dbd53b7e52d97d837be92b5f22e8ac5e8cbb70ccdf41ce8370fc4c4389cb666f6bcdcc5a7b53bd02ee9568e3b3e75ad029c044481c17836904e3d3f8f90c" },
                { "ast", "d2fc18eb2d45097deb0aff8ee391ca95885888ff203220e05d409debfba4cd9b8b19a6a27c45b77ffac3d01efb407b53ca5cef8f7a2ab3d41a5d366f77ce656b" },
                { "az", "08345ae01d7be95cd842e33f9c289a3777f490b4ada7a275e17c531b39a216eb2993b995bb800231aca08b75e18290f8d7c28a1a05fbc5ccfaaa5e198008e55e" },
                { "be", "73e9fa988c1abe861f54a889f7e1901f4da616c436ef17bbfb59f5d96cc82d4dc71a335a129e8bfa8b4b29370f65758b2861b89c8606c23baf545839461ce272" },
                { "bg", "9eb656a7c881e7ba24841c9e5ab05c89210640f0bed4a07ada61c24710855f94792bc265f1bb8573f284c27c29f7eefc0a2c8704d3b288eb7fde6c539e7e643d" },
                { "bn", "8b9b50f63ed2dd8fa6352549236f52edde7ffaf5ff331723c2bafb88c29fc2b5ec853120ffcd5ea61148b6ddd440877fc204011e0e91172bfb4bb908afa3860b" },
                { "br", "5260dc3c514e63e5a0b6087f932ac8ae4a0c3426551da2168320ab22a58cbc658106d333928892f654f7fd03bf3b64b937ec1460fae486f43673e5ddd6b364cd" },
                { "bs", "ed36685cc3c932749c95d96d4a74a298a319637767764c9677c656dcdde2c1e97edaa45148d28a14c809ea40fb08aad2614964950b087adf8ad0b170e4bed205" },
                { "ca", "96ad3a9c22ef28c2589119966f09ddd9dfe0ed3cfc97ec1b31e51f37dc4e0f2ab9d2ea8cd31309c30d5e2abc7b694dd5eaa9e12d6f77d62107bdb0fb8b92e8a0" },
                { "cak", "6fbb2f6195a3f86f197e6d3904c4f10f9d7df58a427db46871fed03143d7e4b5eca9797b8268f65b7788a30f56927faf9ecfa1fde2fe1f63f3e8e6a744b4ed07" },
                { "cs", "60d79ad362d4c56d7afd1ee0dfdccec7f5bb9457be8b982a0b5fcf1dae80869abaff5f4da0a7b6b8057f0f3c8d61a6e4b4810179f3fa72f54020c051c774ed30" },
                { "cy", "2bdabc31b0a2e7440b7f92db91807cea9510eb4026e3e1541871ea078e556af658ecb87cf6a8849662adc25834723e523dcb7a3be30206ef5e2824b06e0dfbb2" },
                { "da", "95a28a8aa7bc7127fa26df12ca5a577977cb5f5d67972281723453c38b58afcfbc7b4f45ecc118471b959ecb3f498b5d4298d17d3f6ce730e835c358310a4142" },
                { "de", "a2bb601bc1391dac62b348d88c11c2e9cd59622a8807ff1abc715f7ddf2c3cda315782f207c146221a4d7e14048e2c15a2ff2086a22757e19a769a67c8b47943" },
                { "dsb", "86731a4d1c0e116b256c3b2f4bb67891b5c7672f1d4988eedf640f6688a52badd90bb9583c3bb55c924d19d931544710b563ef69bd669f1b8ce7ce1b2194f67a" },
                { "el", "df33fee136fcfe9379023b0200f1fcc4259e2b03f280251e08aabdd4fe068327b595aab451364b3353fe034510a37fd13ca60fb6918d6133134d1d6844bfe560" },
                { "en-CA", "9348c4d9266fedf4f12def82c6fa8c83ed3ca20b23c51cd9c6c83268e8cd1c89a63fabd811fcd5337f195b84fb1efe388fe0e42b5e0cfbdbfe2d1720e6a4213a" },
                { "en-GB", "b123d0781e2839dba03739b4d5a43d7b41f493e18671d475578b7d67ec4eb6d4c5750260972d8f4e9c65490d8318ad4b2997df2a4c110703fa3b86ad51e76302" },
                { "en-US", "4bd396ae14d1b1bb73477b77253454a37c1fcbf31fb0a52cb4b365fa635ec715356eabfaa396f2861305e41714ee42ca1346fa693c20b8c6f0be5309334d9b84" },
                { "eo", "929fd0b4509f9f47b40cb4833d78a8000673c17ecf5c5c6d6d50a3904467ca6e7b7c3f2623b2a03c8ceb1d40c060460abf83021b9703a5ad5099e4b3cc3363a8" },
                { "es-AR", "6752546c3ac4f2640ffde5747451f95ea6aa126056e1e25f338c6d2792cfcba9156dd571965b176f5669ed2ddbf417d520eedffbe79e47a7c7b85400ce8af4e9" },
                { "es-CL", "9959f33f85450ed1d8cd62f511de0414dcfdb7a78c1e37a9a3e6fcd1f3e3ab563aedb3d9b680d30c6682dadd67b9c15f118d1d8a07c8f9b6d0c13b18756da8de" },
                { "es-ES", "43ba14217cebd799eaa92c69ac449c74f6efdae47088d2ec26fb7e3d22cdc089a58b6a944ceed5727db7937a40a8074de6474214ef6ee9c64c0a81a3cb06d2f8" },
                { "es-MX", "3ae036bcc8463f3a11807089b5144826fd5220bca52142951f0240c9f12a953b0a8d2552829b663a80f1314229ad0d224e9988714ffdfd63fb02cba3aed87939" },
                { "et", "8f14e469ecbbfa4451dcf8781495110ba018fd59de6e3bc977e5b48e2d4153d97ce511762efa5c326183bde31d5c914ea4285b0301e6bfacd6b9cf9487b92f5f" },
                { "eu", "9defe777f6fdf9db78a424eb49a681f8db3d4a18e455d4003b621f4d85815ad9dde361e534a3bb5c6a2fe5197ad94cba0dae4f61365ecc9b48c7b46cdf77f104" },
                { "fa", "5e4a05660c1a652dc1d267073f0aa8cd3d0fb70131702a35f80fec16cdeb2f375ba92b6eb16dcaaae7619e6c5808ecea3bcfea08bc738aeb3de7d23972b09c3e" },
                { "ff", "4e5f4d61bc66642066498862a2c1f93cd192b70da830314a841dc47aae5b0a48c9e70f326efe87ddb29042ff8449bf1d537b48e742527b85c4ce05abee39c5d1" },
                { "fi", "1323ac8d8f2a5c0947adde749e721ae6ee304f09ba0ba598d82806bd503393a90c0109502e9f03882eb7ed95a0347977d05b31a21431d9a9574bfc2f0ec083be" },
                { "fr", "cc0e73bbe1b6b4730a0f5fa1ee9ac42fa75308d5c2b28e4eb672b963a0b09f7afe1cb9bd9e875fe594add23dfb50ade74877aad3584f7815c5e24d53d86bd000" },
                { "fur", "22638424240a8621ebafe9df33b2729a2ddb4a9cc978035b903ffe63bd7e46aa49fe4020850ebae6b3c4547ba893bb33ab860e541077774dba25c52e99642cd6" },
                { "fy-NL", "e22c9fdf6e7e4d4c29b0a3dc5efea37a00ff5649657610e1f3058326057bcc2ff6a121e8450210d68198e596c5579e90ebbfb407fad1b5c8af5e909e1f6a79bf" },
                { "ga-IE", "42bb1155b4198657da8f5ad3ae91857e833f4334e2bdd631e515130e90727163f4b116e2b2f7ec8e24cad9177702f577cbc08220ae7706caeb240d612108f209" },
                { "gd", "e4f63330a91d861bdbbbaf15b62c28968e8341f37658340b5d1f075f64f4e834b8e7fc4fbcbd7c6a133183744ba981e70b695ff6728d83d5a0b9fd5a028a7af7" },
                { "gl", "b6db133bf8990cf4a0650d8b2fe04c8b898511aa99395c1801c92913218e030a21c2188506675e7c91980f008774c232960d5afe386ce559655e847f13024675" },
                { "gn", "e402b5eace1c3dc025e1aa0f2eb87662b6b25f1cdd7d7d92304c5607a70d9ea4a2e470e7368c49e1fb872d853d60611dd40bef231c9a44e81612b04e6aac5a92" },
                { "gu-IN", "2a1369dbcfae79eedb62b67f280a8b3b7204b6eae22b4a4773858535612a75261e19c1c17e43c54dadf311335e9789f52bcdd7cd648feb0ac0e352a427808744" },
                { "he", "95824d879830c9ff4a5fa0d39db585ccc72d9340ab06c8ea0ba670200fd22e951055735fae5a32efaece33c59ba618242a75578c5279c316027897f0f7e58d5c" },
                { "hi-IN", "e47c7f70d48934051c8d18a98bef09e33075150f7a34b948788892c147b22a63fc7d4bfb1c450b90ea445c599621364b331f43b0b88e70bf7beffdceaa17cb1e" },
                { "hr", "9008b842ca39384fd76bd92d5de372a7170983047fab5bbcdf88250b8b18437caa4ee5a06e760d34bd2f670c4fc5f3e145724ce98abd4aefe1aecb7dfc6c6c63" },
                { "hsb", "b801a523ad1d0def7fd73ca92cb1062dff609930c54803e71e4f7a1aaa0be9c50e6620096587873918563366358433cca4df0c703b2157d0270e28ac1c556065" },
                { "hu", "8751d130a90d4f64f0f6bda1cf7f9c624d4635b7b978523074fcc385966e85019ba560e3c4d754b8d46f1aba9e73062eaf000dfa9b48fe6c000a9eed3559887c" },
                { "hy-AM", "b8f59b3c95de91a1620ec6eb2a725b2052b819de2b4b2f7313f176231e57e676ac2c794dfda34b35b86c7005ab4745820e9b72075cfc8261b70cbad562429a94" },
                { "ia", "679e6f1685860dca667b5452483888bf359b718530b562aeb10351992afbcbe6c4a9f730e70cdde321ed49917c85cc4eff54bcdb18a9c5c59bb73eccd6e5862d" },
                { "id", "8ac8482fc5f36f39f0e9825d5eda95e3945c0cc619bbb68d12bedabeec82d3c32551727297d979b28f4faa4b58501bcf0d8c6cd1fdc8cf15be6496a338ac209d" },
                { "is", "43d7cba36495c953186a7a1a72ae74015947afeef208e0e2a33117a8c32a34e670949cec4521cb2b410487d1e2f3cff2dd1cf0fa8a41031136cc15d5b8bb21df" },
                { "it", "2b240da966d6aab4c198d00d107b318a40bc6191fe85b2c55ad1a58067ce55fbfa1b8d24d367647ea1c1a4a02501108d225c93aaae944fc045409889ba9bb4e8" },
                { "ja", "661e3c75d687243bf149376d17668838448ac2e54a47cbc2e140cfe6a88faf10dcafc7df2a2cc08565f052551843ff6a33e64edb76ddf3faa2f96af9014a93ae" },
                { "ka", "f8475967360295309b26380cfff824d7c31c3fe97517b49778fe5b17eab3381363df3a50de0ba1767b1a5a7e17775b6990bd930c69714198b7748139e1c726e1" },
                { "kab", "242091b9d59b5b8d52a8f239c439d5d4ab7fa4fe0f04cfa3f19fe56a96e656b2307fa4c5bd2bcf6d92c09ba4cbfa8c1be5c48647bde48fa7b9fca62a1f488908" },
                { "kk", "0d05e3779e5a8abb6d79bff1618b5055c5cf8f98762141a05815a2d0dd73769e0efc55e6d0fe5af35145643044e578843406f37b1f915a14c4fcebfbe9fac296" },
                { "km", "ed4781edc2a51676397b8f03bee2309a49e3c05f374d72249d7e3d6a5e47eef4ab136ca308714fc76b5df71628e40b034b9a24486df191a7d025bc8880aba39f" },
                { "kn", "a9c83ba5f4a23d83c0e12b8fe75b5f4f145a04b47a496ef3762b40c2ee9f6f66e324ff036c9b2fb6db0d06e36cf0de8693ffd927e3d617d4c10ac0354df9b083" },
                { "ko", "f32806e119b77fcff83ee05b6cc76db3f1afe192ebf5112913674ab53af0d517a2ac9a412f44298e17ce50cb448132254d5b814e47f6b931f367b445e6dd8210" },
                { "lij", "4ca43485b45042d8757e14f6ab2871be8b854b9ee1114b44ef2f7260977145287ffece2088e415ef85d44c2043d3a5de4247b8b94bf6b60354e6a139aae41de2" },
                { "lt", "02d456471b10952e8b07a92fd2792ecf0742ff39059b7c484cd6975f4685bf06d4a52a38885129c922987bbd9b311c1862f204b96399b120d08eb30b70f2964d" },
                { "lv", "80a90cf79c99209f285f17921bd5a80387051b74d7b7e46e47c960fe70112f05574f0877bafb23275df6cfbaf28bd68920cea09014783cace12771f07ddc032b" },
                { "mk", "d059d865014ee0d0836a055d3bcf7914b9419526319cc591e3367bab6e8e18baf3b2ba71e29d6b6348c572be15ea9dfd22bc521e633f1a2d90ee7c766fc97e43" },
                { "mr", "a366bfa0323ad0e82e4dec916a5cf48ffa632d5f3714ceadc6ae5bdd68ec1d62245af759678d72aa194c0669cee3d7f26480b79d81cc6a361fdc1858b4786fd6" },
                { "ms", "03c7bdea8aae490d69ee5996e940bc5955df0111f037e0d8d3b5a44f618ff94a588d6f2b66e7610bca2c2d2a15e5053cab5f46e522c1a79013bcdcaded9c3398" },
                { "my", "675ede4f2b10163671310c82ab99dcc2e505f0fb80f6c094b8e4f6166d219d2d066c368197118a2b5e0febe1aab64d3c63d2a601d6019ca91b1441fca3d4a331" },
                { "nb-NO", "9a634ae4a55b7f8dc6b4f9a27461f162579b4b8886c38faca904f4ee58fa3e12a9ce7a9546cf3daed9bb33e4d6533f03235fdf88d18ea1c0e24dfa523d92f073" },
                { "ne-NP", "5d99aa9766a5660eee4e394128dcdfd4fe058288f2bd680d9c85d7c6ac0d82abb5541cf00645beae54d89e8250d479335f3f93e56b18c742732a21ad9f5adf22" },
                { "nl", "75c4e04e7fd76466f2bee1aaff7a5238fbbab8044c4828290e0743aab97f3093170848e2a33196ce0f2d31f9283398d186a5b0c77ccc2de0dd66990003ab9651" },
                { "nn-NO", "5bde71d2daf1dbee507aaa7ddf5663ba4ab91b75d479cc3366ff4e0fd931b9444218b0f835cdea5150f609cf7e74157ac5fd439149396ddc0b792e4433921479" },
                { "oc", "b1fab7a3b7c93debf5072eadcc1aa2212f8b0ad264ca78f4bb1c8e77bdabd4733bcbf16b57db70b69196d7e216bf40aaa0c061061a00634cf995d7e9f34d8e45" },
                { "pa-IN", "66dda42f47e157e53f006f797a35eeceae99ee1bb59594f80cde9e581e4ec88fdf25680073ca302fa43ed6b703720c85e52b086425a7e3b7b3ef16677b1e31f6" },
                { "pl", "73babdd9ddada138527b4100509cb0c9c28593341b18d7a0632d132e92cf40fbcbcdaff95daeecb52e6fcf542ae78b22f6e26ef636eb21e69d4d91910291fbca" },
                { "pt-BR", "337eda347b44c931eabb2cea49d6e6e4fa0e6a3341f3f91b8f0e6f65dff820b80ffe545d41eb4e7752f11400468b73ea26a553e2a92f82e504c52e57ffe90bdb" },
                { "pt-PT", "d37e2d06f13664ca3e69a48cbebd84f12d00009d60b25e9a326dda0ea5d7774bee18d42646ac6d2aeb39ba3640852cb314ae8ee7be89784681c7ba7b2dde6e22" },
                { "rm", "831d5ee579c251e1f23d66effc27620e23cb6eca33893dc07b67fdd430b3ec76fb4384630c108efd56227c8b7e9a953b8a9cf4d122771109ff4a91fab2373d3b" },
                { "ro", "6a23006291d606b9fe72ace6c1c676af84631f6d0cbc95b5f9de3cf2636fbd3458d94f4b71382e3220fbaf03ecf61a0e182547fa36c384e8cbec76585d069625" },
                { "ru", "820cfab9014a07ab58be0b0c688b694a2b75a202a1d72080916307770855afeee44fa61663f862e78f768e24000c125b71607d3d9ddd3de2705f4507d85e0b4e" },
                { "sat", "b74b233d8960313b864b5fd07ef5196e61e51faed4cf97b3a8ae95b1473866c758ee6fe1ee0e343db9beaa47974efd2214fab1461732f31452d74a548bf525cd" },
                { "sc", "a1edaa8a1988b3e83b122e410e9b834334e728d605784522b4439104fc525631f28ae9516b6ca856019f41e0d5944bc878360bc5fee8b0fcd51f9c548915af08" },
                { "sco", "9d8ec0544d31921b16e7f42681fa12bb470f039759ddb73fa44ac12e112d0c9f55c87c9ad4d6c2065549ef0401d052419d4027ee18efc40077a1ec5eab51ccd5" },
                { "si", "bd7eeaffa7b63795feab24fb9913613e9fdfb7589869bde2b6f3e1a4485166242cc7687e2db1eb8dc2a72292c8055ddcb465ce2be4981f3134d328a0d09a4568" },
                { "sk", "14612a030800e57f7a4595da0c4de4e352e693fb82e370a134471ab5700d361c0a56a79f481581041c127d67b23b66af45d46fc0bc9a0fb679bf9e60ca1279e1" },
                { "sl", "61c92381574928ce55304f102ce59a21447515b01764a5f4ecd09235c77d81662b4ea2b8ff6363b086475bdc5e56661416b5e46ee26aa47370671fec4120f7d7" },
                { "son", "e57ca54c60b43750afacafccf9f062e18dc8b5419d5b5166e08549f2f9c634b3ace844dd3be80c41620414452deafd7ba2902f526c1648ac26cc8e7baac80f1f" },
                { "sq", "fd4f77eddc812e8644475dd94bbb7e23a5fe3d01a2b0ea8449ea98d2c683c41ec61a1da21a795f6a6107d0b45c1bbb1ac3e10321798181f021bbb774976f04e1" },
                { "sr", "00d2cc7733a18fe775d6e8da394f85ff12a5873acdf708731e9b66a87e62fdfeb1f89fc417ceacd183c3745e49b75c9c87199fc8b56358c1d59b6a774c7f5003" },
                { "sv-SE", "f3943bacec725cf34f300ca1b522098b2e31fb1946bb09472d95879b15b2caf924396fdc5f2b31f204d4d5b5f6f7eeb0c1c1eef1be08ad2db23a53724cde858c" },
                { "szl", "c759f53800f037627a10ea4bae128654edfab272e829cc20ed30fd4bc61542b26d6d3f4251a04f45d3ba83bb6e7939672208bd806a8d156bbfff597282969e4a" },
                { "ta", "9efb5ce299568dd6d2dcaed404c0da25917868b63964448c918e6d7dd84d34383779a7c0e730b9590e821f4003eefd7319562a5215faf23c19792e07183ddf07" },
                { "te", "bf4fc5b28a4fcb1aabff3511e16cd5a655498fea9902ddac480da4b8f0bec729a974a0b9b5c2968bf192570182be905f0cade564bfad09825ff71a69c66bd01a" },
                { "tg", "0a5499459e7c8d5c23c1b5a785db0435003a5b01cfcc0693a8f075af3442055fc79f8ee626ad8ca1b204e5a3b4f5be7d532f0664a2cc7c164c868b2a8cb048fb" },
                { "th", "b8d08b8fed9012b502380a7011a447e856c250bc6fb680780ecd60bb55961aea3ebcb3a021fd6b51f1625caa61d5e736037967540ef4ac978c5fc2c1435f0386" },
                { "tl", "6edfe8119e36e074d73231d8cb2f61335bebe10296e97168647a9a9bc324b5756a3cfce77a583ffa35aa6a8e385860739f6a130363e38a46e289e9d971254e36" },
                { "tr", "ed70d818e281cf7981def5d1c813a8d1474df63f07c7854593e3edcdba8f63ddb919948c18c3d51c5045eaee8d1a306c307c6f282eb436f36b571b9c93e4e9cb" },
                { "trs", "3e25c29c64f942527cdf1e8da6aed73af3200a0f969d3a51c14628f86ed5484226c81eeb64b6fca94172ae258058bd215bd712b081e1c907f2450a7f830972cc" },
                { "uk", "658deda99e28601da10534e4bb9c17566e4ed5b6d291cc3694e734dcbdd7cc423af9400e576309c0d28caf37725eefc109ab19a30ed471ae612e151579f7956c" },
                { "ur", "184758746cacca8842638ba64b080439abb6455edea1bbdca6c6d7e2d2fd4bb4d833fd566ba634ef6a70e85f57366607ba05cdc7df22e9a3a801556077d11cd9" },
                { "uz", "c56e0b4c712e9c3894511c8e0c960040853c4bd5dfbd281b369b4142c83156d0fbcf3a2b895b997c6d0b617a47dd20e599fdfe774c48ff590fbfc8c3bb82eede" },
                { "vi", "04e81d7d2c49ba7fe6397fd1e88fdd3769c3427507c57e0516c10beb72a23599fcd4d7ee9dd3c037c263240e40ac0af1f18e4350b88035347c90fbc36da416fa" },
                { "xh", "d8cad1ca0ec81cf177059aee5fd66746cdd6f02456bcd9ccccc4d297534df4fecfcc883cc2506a189c4773fc1494b5073fbe756b37d0d78fd92f144563042099" },
                { "zh-CN", "9cd72bfea0d896de6d33c74ae567d6843dcda20e728e083c9f7dbe18941610201c4cb40a134b7acc4c24586947f298b538d8e9b143117fd84bf22ad31ee8e28c" },
                { "zh-TW", "ae4fca83535166f7c7676de554e45086ad8e40b71ec831a1a939e300561d9f2997c50e6f7c93a72ff69b0a85cb25f5e031fe5d81d5394c90de263c7534f502dc" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "121.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
