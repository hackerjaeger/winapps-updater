﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "129.0b7";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2649cb6bb92365883ecfe60373fe025fada88fc97b53c3735bed1f71ba0a3e1f4b0f0c70a99df2d53cddfcc67ca7f86092a9f7cdcad7d8ab165888f5e84701b6" },
                { "af", "b0ab7325155218793934771f3a260af28dede50a369b75bc584ab0c32366cb850f457e9990896d29a594543220caa70f9bc7cbbc1628772700c1616cb5c8f8cd" },
                { "an", "ded6b6b57ac63d0d4f5b67656a53989e57a8787bd47b14459dc56a3f349c7a90d6582dfcb945b76be9526cbe72e62166eb50cca479dd1a53a2aad59c6beb7dc5" },
                { "ar", "06119c8b2f4e44c4fbb17cebb702379bbad48bd1036ad9d42346d2bb8fadb97e649b2e822abf6c3934794d8e089782bee281792714b7fc173711dfc999f35903" },
                { "ast", "87b93f65edd0568a55ccac46e531c85e927367a3ab74ae496f8be6ee3b8fc65f9cf39f4ae362b9ead995d96690e1352f133c1f51aff366ceca230b7b116d6927" },
                { "az", "b7de58d3bcdf5959c127e5ff106b634d240539c0536abd9e891441f008bebad316569a14696a10cdc6bb9072d08dd1f0b39b86282ccb64f488a8b090282bc264" },
                { "be", "4793361369d50fb3b8c7b56792c03d5b39b774329d671508893de27b254762e16042f717101ebbf35c5ad2a2c6cc671ad992503903f68b3c879b32cfdb2e7852" },
                { "bg", "30c0120befbeb89f9ecea515e191b74e4639c611f8ad69d6efb96265cb091f0f0ee11110a20392e07a68322c91c0ab7cd1e7e3b44532d41f10f7bc0877eaf235" },
                { "bn", "10b5ad4a2efb43fdc80721cbfa88f31f6f34fa5a6751a2fa538be49a1b39d64ae9edf890cea985d26582a09f681a9b5106f13ed97db1abed9d89a4656d76e3e5" },
                { "br", "6722336d208b15a9d5de0b24b7858c31b32e09675160882c5980f7b38efebf35bdd7a675e45ed6566802f1c4ec38fba564f81673a3c666a40351aef3172b81f8" },
                { "bs", "ca553b243d4ac51584aca1599240da62bcae4446733f9e1fb2cce42d17d4bb5f43d26365caf98fdf82218f0432e7b9170cb494e5860709f4767c3892796b399f" },
                { "ca", "31dc8e873711669674da87a060b27234bd93978ac9e876b4fa99476f740723fbe8125368afda9b2e2a386487bd87095e059f06a372fa32499decf75564d6abc1" },
                { "cak", "6f787ae05543270960ad4e600dfbd13f7e601ce433a68306449ed6aa219754480c4c077121edd8b863cb890174f5b18f603f9cbdad06f39e382a43360e88455a" },
                { "cs", "f68d848631d0d2c81899b8325c0dc66995b8056f78d46d2c0964d005fcf34e0f6072380c35cfa1e63ce2448268b5a7564aa7d59013744e9b27a6e3314f6ba7c5" },
                { "cy", "4dbac6a5824b533145579eaadafccbd197e9a92f2725cd4c2d984be7bd8feeccdcf1ddf9ac1de330dc9643451d36e011636e2a059db4ef5d3b08e301c739a41d" },
                { "da", "87e14f61edc9cf696ebb71bfdfc67519a9eab7e064ef4134d9b72c62ba697d59ae7eb737d2b8d3bbd04703fce216156b942a95590ad994548a33ed56d5047dd2" },
                { "de", "13d18edc6dfd272dabb9f6f4295c5bc1e32ac6c9f76e0aef3a4b654568edff93430b69801c8bd9a093cd61938d2bb98b2a74f38d5ac98630c658326fff20f570" },
                { "dsb", "6471c741e750b95e1241ed181e679c453927d29aa5f8842b2ddfc27ac143f07d6c40764a22dd378823b1abc3febc18d9ab39dda63094db29f982149e08bef2b8" },
                { "el", "656a637f67898ff0869143df89d83a8e4e32015e0d548a5cc295d36c5160f30656d4ec048f8657fcccd5dbbf8e5935b4fa2389357b0971b5ff774a9b937e3c14" },
                { "en-CA", "864b6577c71b13cd3267948c77246aa5bb9a96865694ae7f0f5ebc23f9eb46b535a018d6fafd37de439af7f57b804fc0fc0b1364c083eb4ac152e5768be5e3ae" },
                { "en-GB", "ce1f5b17b0cf8c318b10aace7d3dba7dad710cfe4958ffbd377c76d758f08d5ccbe5be374ec4a6ee4b01723fdbe2503f0370a30bad7b5d01979e667eaf41fd7a" },
                { "en-US", "312d96e92a9492d3f3783e1915d5e64c85a1c1c5516055e2eaf6ff83f473416cbc5c5d7b50e9337078b5e1f7d765f948ac149724f2206a553e0ab53b61f96680" },
                { "eo", "f8641eaa0239bcf3593950940b32bea174074aa9fdc722b872eee0aa036983f7249567ed2c7d7297344ff8050ab64373086adc2911646159bc01b3d76a0228cb" },
                { "es-AR", "993a0de1c2d9551464eab694a7768130d93ccb6854065cd35b69c8a61748032f768f95b48d557fc669efee36a3b57eaace739cc3db10124d3b02fa618e094d6b" },
                { "es-CL", "819870376150d72c61dd6fe74616f81d1ec491e2264600f13f8deb299c6bd777c3010dd427d364b9b179f963da51d613d46e239671bc675924d7583e44056223" },
                { "es-ES", "d1b3e5a44a8e42cb17beb39e7b29dd1dc7655814a9c8762ab22af5a086e20d110c11537e349156f2aa7663b624a7c238210666435c37ce9e2277bedf1cf3935d" },
                { "es-MX", "c7675702067ec729f4f7905207af7c86f1ead98b9c4b749d69bc3da1f72dc335f326d4808d7e0f0e54f737bd1ca0d051d3a2ac47552859ab9a70c6e7b1e1371f" },
                { "et", "e6b3a51633a89d9e96530d24eab6ba29d8aa86e1514c9d500b911266ffbdb200bcef6e42f13efd83a19b9f4e8deb537fe1c1a460eb6faffce3eaf144842d9c05" },
                { "eu", "1488a09f94177c960d8f27664eea59cba356ca41b087fe3f38ffa0af6359b3e6b7189e540bf675f4fb18ac577436dd38c683e524c507a1e71407fb15eddcb6aa" },
                { "fa", "25e70bcedddd497c9be4782ea55ea13a927c973cc38b999aa2e2a9848f4de303fb18281c0ac739a08b11bb1dcfe98665701e95be685794c5a58bd262f59d5379" },
                { "ff", "19a371bb6675ab7aa62f0ea21b986f7333f8510dc7d7dfde47dca83a6c60a2fe7e79ef52888494d812bf2d151c10af4c2bb05dabab7f15a6fc47db24b72d311c" },
                { "fi", "d4dd846037cf7bccc62d3a4c51872c51e4f4006fb58f8bccf4bc2e22ccad1043430c47b4419a5ddf96efe0808b625476844ab098857f8b8f183710f68cdfbf60" },
                { "fr", "debe2d355c16616969a5bb0f969c953d22405dc581746bd3f95b043e96b37f671b20fe5591cd0e3e0405d7e6be01cbd5ae42ec9bdbdcd219130c8da5a12038be" },
                { "fur", "9acd2a124dbe1e6ec0f06a4aad10f988d278ae81a5129596c7d12dd52962b042315eed7e97c2202c07c4f1c0415bdf6f231f002486859daa45eda057af2f23bb" },
                { "fy-NL", "edf8711b24e5ac83d5651f091730ed58556a6c547281f3acacdd73de8cbcfb483ca54e81bc0c5dd4ba41bf6c24d3dcd661d63aa119cb8983da09729b06f6aa54" },
                { "ga-IE", "12843c5f5318b580c907a27b75b5d98a849770a8c99335d5dfde31b949335a88d230025ba14d4d73f3611ecf79298f526363e78e6dd080d0834dfc61504d7507" },
                { "gd", "7ad3f47ac517d547045fe81d1bb1eae3726850a408863f0a48d393e588af206f83a50cc04b7a2041a6673a49e596d7f44484d76939e557c0f3d3cad6eb306abb" },
                { "gl", "ed498f0c09144fe315de5086afcef5edea18612579dafcc21c242f5b18f7840dd047dd09fe632f07f8945297addcbfcbe7813a90e906ebbb0befc5467dd989e2" },
                { "gn", "e069e2ce6ca833db3c2c26c2999d11d3aa1a168f51b91f35a03edfc0945097644c0297cb239b09685222678668a93afe9d0aa7d82a06e5b04e1684ff1c2b9bf0" },
                { "gu-IN", "39fc57b1b88646197259df826fe298c55d3586fe9e85e65ce448886aec4066a9ed4c0d45f3c882c6ab2af38f095d86f89cf60941886bc159bcb45dfac1d1b56e" },
                { "he", "c514e07d9f9badefecc249cb851a75202b580cf6ff0f3fe98c47473abcd00c39f3c1f545dedb113330ef8f7d60b5ea3f45fb3da6544fe4085fc772cb2a507241" },
                { "hi-IN", "6669ee47adbaab5d6ac5481a5ef9012722ac6d9060ecb65ba297f973b1946ff2421295e9b49423374b11fcf62d9ce8d8b48d13ba6ffda8d6c4207d46f6f463bb" },
                { "hr", "46e9076c840a28d7a8f781059bfd9d55a806031f9b3b5d51206203a1230a98aef20dbe47109d29b0017a9ce680924628efbea0e4c6d15d72ab95d74454bc8fe1" },
                { "hsb", "46c5ba57c1280b4e0f1913498ee599f16e2d781df1ca5edb7b584e51167ecc70c28cbd2109b06e83658268d4257c7bd07ed3f03ef1dae465fb61a72eda3992ec" },
                { "hu", "e076b9c245e0e2313aec03e33554dc55b0cc9e6fad52e8a2118395ae02b3fb24cde3d99f13757d63a703bd9ed0887efc9b52c0d79e04bd931b9b5fbcb8a07dee" },
                { "hy-AM", "c8fa57d34184dbbcfef40d84b328e9dff66f5ccce58824bcb96acdb9465d8b94a6e3f6a817fc07debbd01889554f6ec24a1220f10b7aae3b0bd9f125192e7b7e" },
                { "ia", "fedf96c64ed435338f2e8582680db96dcd580e055e23a536a012a61acd0ae7c035f1669ed954354f57e17fd1fc326181adfa3dfb691d73b5bc424117188fb0f7" },
                { "id", "b79b4a4ab48a7717fe67e78a04a9dc47a167f381fcb2b44d14d2c974dc61c5c194d13be9c3f7f704bf8f2fd45368db93c7c864578589bdc9c2423712149859c9" },
                { "is", "c36dc668082de352794426d4a99404fd069c4c766083c7695201f5c3a50447887fd7721fa6e42befe7b811076469a5cdcb6431e6ede72a3a578a94eb659beb2b" },
                { "it", "9b5ae33a3022c35f682774d11352ebc4645d194b2a2d6074ff24548f4f620bd4139f2d91d37aea11ab6035b92822adf1655a6667b965bd4f8342c506f2ff5fbb" },
                { "ja", "74819606f74381116a12192de5d3e5afc9d9d6fd3b2631dc077af35c501752edeee350b6dfc4898bcea7703f7f83f48a5fe0b9d3cc21e8306721153416b01723" },
                { "ka", "3e47314172c90649de1775625196d0516ad4abe01f2d2db934fa3c01022bd7558b96de33aef05b69772c262c726614c38549e1b0ec16fdb1a920bc7fa7e08dcb" },
                { "kab", "eac2916c5255d8e15e598772b6d068c6e286ad90021b2854a26552ebc7f857fab74f6fa2efe68448855697d420120b8bb2b68e2e7229c53714e70a666b748cd7" },
                { "kk", "c93c6b953e9f5418c2bee7c001a197f98f833f5f13847340079731c04b698e53fd4da9af381e457f7df3be77cab9e59912d43fa7521ba9a81576de45c95affb3" },
                { "km", "4b2e2c5a8af892ae36c394da52c35ecc40c08b92641e697600b52d86e6609f071e73fd53a9992b0d6835fa13697d921bc82074eb77eb52072036ac8f4d3d1caf" },
                { "kn", "e2f67a9dc1529ed97f2b5db3806244f79550bff4b1bb1acba29fdbe12f5fd07f25467217b82d04a442421b7273355a104b1297c1f77df674517f279c494dac32" },
                { "ko", "ddda6f6227d2634a71bb83bb8232373577308b554e8624100b528878380b839a6fe15753d209903984c4101dbf5b795938bc7dea80833b91b196084ab88f0beb" },
                { "lij", "539c2147dd627e714e433fed6c50d12b2cd7458a950bfcf66867b006b0ab86c8594203d04da406677f5ea64d51ed3c4c44ca2cbe10f794102eb335433f17782c" },
                { "lt", "6d8cc882f8f719a6a7c96161e7b821721a81337e43207230f175708d931f174881f8e2258d85a9205e97df139a76af8e0e87c3b6dfd7fd8c7a16dd7ca029d0fd" },
                { "lv", "2d1d56220e7d487001a0bb87d0d9eddce4f09de6d54c1efa3f22b6cdea2936fd6c5e3f1076ad43f8d521cc41ba840bc0eddc2b24b36e3b446c528aa158b9c429" },
                { "mk", "4b0a37c18ce0b67aea6adfbfc4d1d092f85e7ea0f696e5ae9fb94217f700ecd8c9042fabc07be712dd6315a4d417ae9e3034d326dc447c631cb932b1f7730cb0" },
                { "mr", "a9034976f9cb50820ff19c09a601213934520f7347028face1bb2117da92843d12e53d8fd34090caff6ffde7fec2ee4ab96935798854c3dbd12049db0a933c37" },
                { "ms", "b0c033779a0a5cc41885ce76d54ad6fadbc34773505bbbf9a5a9ded95497cf9bea5043aec561a93fbf19c77b0393fa32bb773610f0a9c8564cc2ad9e508874e0" },
                { "my", "e5f987e3cf0083a04d838252f786e070a5cd352ee9e3daea112c7e64d0bdbaea53defb7a4d675e773fe12090f553cba0d509be6157c9bbcf70582a826f3f0a23" },
                { "nb-NO", "32f6058610a54fe5fc830cea23431f94c4ea77c54e4b1895a48d59d8d649041cf1e8caec6ee77db22631d949f46fcb4b9c36197be03bde41a644c95af61bdbef" },
                { "ne-NP", "cf708341de4c004f9d8a66a6c9bc353ab034696a30ce98f7c4a4bd8e04e8f7dcb9068deb58e0e54bdd1eb80a73ce29e474e4d1c02913134243fa0d70284f21bd" },
                { "nl", "7be8159b14e9918eb279dd4638151758e64522355966f3d047bac86d10172c4cfa5beb472d4559a7dde158b27fb17adbf802960709e5449cc3cb5c1dc407e11d" },
                { "nn-NO", "87850b27dd004bbe5a03b402d6ed4d467bf89827155cd6e7546e2edcf303b4d3baa2b15adabff4623d898e9042f09d7fa9702e9ceff963fa039f52641eda415d" },
                { "oc", "29c65b74f2793494c548e2e3516688ad8b255a5883269f91302d892da273d490308a8b9495b3541977ae30f5f629ef727e074dc7df12b85e0f772987f7d668d1" },
                { "pa-IN", "19e0f4252ed2643f1f129506467aece7524e420ef90a62f3c01b86062a668132c075ef0eada65557cf3af4f905e5e53d5f431cb31dedd8324d1da890426a7db3" },
                { "pl", "50e356e5874cdc91fb3eb9381d1826e4aeec8fdf44b41e134c77566c41935bb9dcdcc36f4cd01a90ddbb8b59c1cf7d683c54c80512492d8b33f0dd313ac3c68b" },
                { "pt-BR", "fcaf01a0b63d8e0112f2b361288e62d6d09ecea1bd47d261294a644942fdabfaad38d44b94e67a73e9a4a68d392e0a2224e115203ef299ea50712dc2f67f0cc2" },
                { "pt-PT", "08626187ac306226715b87ca2bf165922918c4f07a72f3c7790146221a448c309dd3d5b1240896eaec81dcb2a8261ec38f8fb9db3c185cb99e484167528ea5a3" },
                { "rm", "a98f0e80dcbae57d645536753d74237ec674a4ab09391376c4c70bde1eb1be0ded96206e15da0f14ddb695c428910417bfdc023e9ae7bd8cc12a6291094c14e1" },
                { "ro", "df63b0ff0dc290c5bfa71c5012a7af35dd25346d1fa7e1b19500c69b56fa902f52f0d12ba405339b93e35b6e1bcf01371e5e4b973d4bdf9409da03fe20b964ee" },
                { "ru", "9d5d85a3f95d7e8e4e02fa5c11db4e4eda15a90acc5b15ac6c45f4cd337e9e3ccf2a5f497a5d2d8451268f204e9580a9e43a351fff068f86bd9c8b038e2b4952" },
                { "sat", "ef001786cbb3a8c1318e497f2ac6f3b7d75bba256b6524978feb04f0976c78780096cc22203dee3e37e4b79b51755c4e9f43efa9919def83992eae58f5e8f534" },
                { "sc", "72a568c6959e62f0da07d05347de44cb2ef1f2b85115186ada6a3faa63c97f311059187fb2aff3aac59fb3558d990d33bdf7e951320ba4846f0390af63284569" },
                { "sco", "868133b9360c508d4118e22e0c118d209dfa1035a03b23d0c1dac9a7f7d27638149476452b3596f73761b117474e9384a7f0ddddabb1bb8887f0be7a56004039" },
                { "si", "97295ca83e86f858d881f7cfebbe3322227b5e3e11818880adbb73ddf4b2c513eb4197b2a8cd61e82a18a7f645004ba9adcc6936c7ac3c22cb5e4825588a494c" },
                { "sk", "4ee995d26400d7bdba3ae583ee2deb2b78bde48a813a1a7a04ef8e9c625bf474a985b1956254e8cd48832f1a5d96d867a14fd33d850edd8b260dd89e87653d55" },
                { "skr", "3dcc74cac6001e2bfd6d1e79e785d8d0eb837b2c3c9e188730d682657d3276fb748224d7fb73cfffc0c5553806eba3814d7a7ea2c8e181c7aa7d35ab76e2fee8" },
                { "sl", "e1ad8ed0dd81cbfb903abdafa21cc7a9198da50f19e84b3956c625e54df9561024b3d19e84b7b93367c8f7bf14ec2ab1844f1746998fe10335bdc68f68333b1d" },
                { "son", "aeea06813593cef9f91bac3f78f08855518bc54188b3ab13a344495d6643e9a5f62089b3480bdd6b9258001b61b153debb7380dee1c07a8b4448dea1f83899de" },
                { "sq", "f0a7fca0357afe53050eaa483a049b46b05be53493c89b46711dfd1c591aa87a6233945dddfa5bed49fb43243a30201ae26ca5f340565f77ba2f8f1e09e19638" },
                { "sr", "b8efa5f6dd3506aa861771d1147d959a63f018b9c57c9c54c2efeb26a3fe774b3f8e00552ad87cea5eca9119090923b31caed79e0eb7299fabd5d4ae51ccc03f" },
                { "sv-SE", "b49f3ec19e2b02cb4539103f77894190e62a6169e07b77c4781995fc815e36c80fcd530f5c71174fd080359e316f20db934576febdd229cfaf63aab8e1bf1e4e" },
                { "szl", "1e0699515dd10ed644841a642ec88dd5e16795454b3874e63bdc14a1b41d9c62438582306a7630a01b9927d77604527198fc3fe46fcf8dbde572b0f0d56d4310" },
                { "ta", "f16a4587f11873080ae24165c0793b6ac484b5168b7c84e50fd446796ac37d680ba1221eff166790420e5f0710391b7de3524fbe8054a9aa233433d934a6f788" },
                { "te", "1071393345454f64a8b1631e2bc6905a7e381feeb7378519e08fb02d2970c116d870f521e3180d36623b7b873cbf88bcf47af23ffe5bb8115557b196c9165048" },
                { "tg", "20e2c447ee8f6a7552140ed3d6bc39b2e130727a17f6a813d13c8eb10ea4387d078aa6b94d7c1c3bba8ec9a4f599f15869b50a9509ca577893cf97fa10b94cad" },
                { "th", "f2867b3a83b2b4f9d9eeddef80b6b39b25e10cda9144ec27eea7f59d67f87629fef42ab0b04c15adfd4b6f9464c62152e4e0b972086c8edb743d5f8375017bac" },
                { "tl", "003212da0d4fa4b0206428d8b7868dcf8351c3165d7a32778b43f8e2449158bc2ffaedf793d336826f2fc7dc929a17f739eedc359e7c8644260ab1813de72899" },
                { "tr", "770a24ec567ad8c52fca21d86ec9514c1118b854994d52c6e97009d7c54436e7ba52b3d027d1f778df8454e810c3a8822c1021df6424b1b188aa9fb9328fa9d6" },
                { "trs", "03ed8a973d7c8c9fed66d0351377a9568b1e5375cacd87c3dcdca3f35dd2f624b855923cde207710e9621015fc41a84de0bd9375da45aa34b7021f72c8e9a638" },
                { "uk", "9f84caaf682bca318f04b578bacc780800a93eee9bd60342b344b7983fd50f44ba6f62501f21eb537cb06646c2dab742172687538d2f896a285582190ca51115" },
                { "ur", "859a9f5e59fbd3b8da1a8950193e847aec1361d5f0eb4146b63b267bb3411e372617f44f19bc032f64ef5da65f2fcd11b7b377116e7fc8e680ca5815ca464df0" },
                { "uz", "f03b87dd72ec8bfc6f4a0e5bb4cf144d5a29ec11779e4d9ae4c03334af0ef9cb5f11e4e8cc2729dc06ff6df7dbfcdb7ea62d67614fb2afb9511f1b7b6ae161a9" },
                { "vi", "1a9fe9acd1f4e42d7a6a533af23c856135f0b58d9cb7409e94707baa7c171a853ff1ee439dca3ca3e37eeaa05b7f485cabb3db8eb08e84181c6dde4654bdf297" },
                { "xh", "2d8796755dbb08d9b57734b11ddcd10b0251de184b4475c57da8d0fb446c43bc73b985c475939932f2df3339e01ff3ab98ef36a6857eece1d9eeafd6473f5d4d" },
                { "zh-CN", "065e5e1bab312d6beaf3e449a27800a3a8ab8cfe6d9acfc0f33f11992f03d376ece4325dc784a07c2af4373b0f4d5b730c5ae6444213666204423230f52ec323" },
                { "zh-TW", "b8e7aa031745395e46b8e71851e89c94e8be4a0da5c0e08d76cffa649bf58f6c7a70997f8951f2904812b253816789b94f3c853adaee364bd7a05476438662bd" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "1c204d45d38ef8b2b47d0df9e240e16583ee6ab6ea83f173c2b326a48eb6392d465349ff2fed404a3029ddf162627def7ed7405feaa24e365b050af3e3c293f5" },
                { "af", "39448dfe5b82ca1d01cbed4d9cbedbc34c1c2d4bb215dc09cea1d4a86b3baf40a8ac2e41f8a3dd137407bcdfdcf55a86797d5b57014314290ff800ee9c0cb85f" },
                { "an", "5eadf210075f2978cecdd3829359b85c74fb64d411e2619e62986d3e64d3623befee3b7b0bac113805198ac8613b622b921b71a007b91cc968648971080cbd9f" },
                { "ar", "733ae3ba2906bd6c9adf192f5b9723246da79158987b9e3c92a5b1be0d83d9c29a2d158efd8b5913c0c83305c5f16ce2438ce0b498a4d0cebccb3f6d92832b0b" },
                { "ast", "0096ae078cc43e64c613298a807f47bc899fdbaf1ca23e473145048f863f0119078af5e1f14322b70979e4ff7bd0c6aae030b73c3d39754e4464153a7ff06634" },
                { "az", "0c8b0f476c7c2d2b864fa75c03ebdb4a6293fd12ba8a5546c6245d2ce3bed7f53b109c11b8487ffdbf92ac493a3400204d7ba681524cb7576541af35501e16a2" },
                { "be", "20c4c299c4849a218a2a53176ae4efc656b8a90bd0513abcc7baea22db771adea7630b14fd17f5ae8046df86f532eea533c3bdae44b4981de20afa2a895befa9" },
                { "bg", "7f87af33da70ea1cd6b1ec05b5c5e7a48e5fc82bff31faa850118e3c86325e7a604848583c6ff292b40b47ebef4f126454ab4c76f5d5e62f1a2725672f4e5224" },
                { "bn", "9da98cf6ade5365720c4ebc718a37d815922d87fbded7aada2040eda7c5d67b83955ab2394e10e04c01add2881f5e2fad4ab177531eda4a70860f3bb70327da5" },
                { "br", "69478cc672295a6e7a4d50e6698224bee5bbd961d30521a8eb9e6b2f8e47a4792d819c98e29722b9a900fead4e2b43adb91906ba1f77c9ee15358924d30a83da" },
                { "bs", "6ccb3091557b860028857435838070ecddc879f7d9a029d1a2c9dafe01facc84170ec89ef9db26505628ad3e0f8e38a7476717cebc5f6cd3cb2fe6a99df29c53" },
                { "ca", "94ad09b562af8d72f61987774c42f5c7d4b91d04fe4e67418d1f1c8ce1ce9c1a47a7fec89540702db04299c6dbc739851011cf35566fa6d2aa025d2dd1ba5560" },
                { "cak", "659c205a8416fb922bed50a8891b9e78b39aebe7547de9188f01795e72f192a5275b2176da9e72b4a14e76a04d581cdddced1913d9d828800548147c813f858a" },
                { "cs", "56d6575527a996e0c4ff1da473dbdcee28d993fd8e32e9de1e4f9fe0e61c9ac5d0b16f3ecfdcebfa796a58f4814f986d58644ee564919dc66e5ce602f5d3703d" },
                { "cy", "f1faaf2a8a6f4ed77b0d84484057895a6243a1454b4a0f09d812cd2cd4218a059eb8e526c3df58e7899345185ba505cadd28b642c03d52904037dccf234ab05a" },
                { "da", "99d11000b2439534eeb85a9b3f1e86f31ac1b5abfa5942898cebaca06e4ae05842c3542be0b1b01d51da2ea71f9e26fef3e3855972c476d1636b3e573668535e" },
                { "de", "b9b0a18ec05bdec9dc93fafaaf4f817c8a5c34df6681e023e5b71acbf193e8f6cf2347acf0e647ade914819b347f1db8982147e66927f70f3ff043c36541cad7" },
                { "dsb", "4c0384631931896551c38bf4e0f5c66163795fcd4aa1aac08ca988692ebd40685e8d15125c786c02f97148099a89fe583eb142eff9e6627818ac0465eaa04507" },
                { "el", "9f0bbee631a179fdc9a8c23e0f2f4d8aa5dd87053c989fd79dd9d39234abd3e06ce91225828318248a83f85ae609a9c3a4cc42abeb213ff8a667bce065a1fe55" },
                { "en-CA", "8b1c7c7350e4dce5c88aeda122a7eada69be42077d77fb001ffd6b44e93e0e61e52a6e574f79140b409083e7e8a34d6593bf98588ebca2583ab8aa75ecf6c2d7" },
                { "en-GB", "469bda474584411fb88fb97db3806e3023e9262073a6c2dced6f149ba5ad810abd5bd7ca08547fec9f78e924e14c57f034789e48ae4bba85979fa0f3f2fc96b6" },
                { "en-US", "c50c7594ed0f5f94cb47b83e0a338e0d34ced12f5b5a6a08992800d7eba20c686c84405707af99c680fdc8cc6a37050f3fe820aa48d43aa955f25a956077d731" },
                { "eo", "3f5ce3e84a61c40a31145b3ffe172a250aab8774c27015124171a5b538b81dced87e57c877768604e28ca96186e5d22ff71855203ad0342560178fe8d3ad1432" },
                { "es-AR", "17db48e2671eb1073b7c2500144f34775fa145b36dc9a8b76a65dc10116edd17f93ce153058b0fd227b3763e35cef582583280b3a5a04e05d477a8f424f3c548" },
                { "es-CL", "6ea39f081fa889466daadad98f6210696cd4e00c4ef99991431178a971695ea718698fc181f68701bf0aed57ea00f793a41725c193b4f135ba53f5495023a193" },
                { "es-ES", "aff38b0cf618770dda4dbbfaf9f195003ed3b5dbec7f95988dbdb81fc4f98e349f520fc644a90480fd8eeaa9320308111a74dead9b70aabf8cb4cca872996b41" },
                { "es-MX", "9ae822d8c31e6ff91c84f287b975515c6291e2600dd3e7a74d018ac3bf05fdd69eb1daa418c1a93f308a7a5a22b91a52075f11f80d8ce5ac91812168c2e1c407" },
                { "et", "f60a8138647c97a8a5d8e953eb1b5ea198be32d88a5ee4810cb0efb0e0ff60eb928678f371c30539df263d8cfc5c35f0cf1b43fd4eee2fe2e1dbf17a725f1f46" },
                { "eu", "01c97646de0aa114b5030635b5d452dbdc6fb07b6ab97ca81eef814652817b2fcfe9f6f0a49ba5c49697b02aa8196a745bd93aade97f2c499a3f0fc089c3c813" },
                { "fa", "7d06101af75d58c37dc2ec9e72a03daf388a20540533c4bd2fc76f75766905612330290c77a1b8d6d6781d76b4f20d0036ccaa160a4e6b003417ed0a99426af2" },
                { "ff", "2e410d15b9cfe8164cb8804126569f1ba5d45ab44435948b94a800ae03e3fd57012ac13cbafb1512754c83e7a6d81f5cbc896567860c0ab44f22d42799899c5f" },
                { "fi", "43335a3e3dac9026fd2093be5ce9d76b0743b3a5a3095245ffa40a3b66fb982fb6fcd37a36a77a10e7ed33f02c9d79b96a7641599691a087710dcabd63fb0509" },
                { "fr", "ce94976fe813d2a2b9aba006b13bd5ffd6c56917c7b8e65e2a3fff59fbd5c22d62b85164c31cf5b55d01654169b0b2b0e74337d022dd47f5e34730c4866c24c1" },
                { "fur", "d8cdaebc05488199a26e23463cf8a7612b4503662d4c67b358405ef19f9ca04af6651ee8782afa83659b7e6eb647aae105e4a3ccf5a12bdc9a8902274bdfa414" },
                { "fy-NL", "1385e57881e3f43a4587f2caf9cc4174ecc7513e0241b09c749fc014682109b14decb3e0a3fa25188037dcefd9439afc11b09dcce6e051af9c7d4fdb40a21a7d" },
                { "ga-IE", "fde2c95b982e62c52e924c05fbbca92c75a0860d41fcb3f1854f5118b76bb26091d5f10d9134072a8bd9eaf3152528485e167e84b8160a920dec8af1c3ac3037" },
                { "gd", "96eb769f1a27170a5349a7736d2eadf260b7ef4429b54cde0548eefdf974ca7429918af1e6d7253fd88ddf43c833b43d63a6d93bfa545ccba80fcfb288040b0e" },
                { "gl", "4471f483576e6ce10301db508da8daa1e52f9c1cd6fd28c279d70a124e533d8188e555361a024bcc1d2e091299d303b8ad51122a8b7293fdb7288eb904691c1d" },
                { "gn", "26a42d70912c03a0bf66f59abf555243b2353086febf93e48cacbd2084b0e4d327b7150dc16f346c22819b87cc7c5d1666f9fa9c2f2eb09676d4ca25f78e0d9d" },
                { "gu-IN", "0d80d6eef8ab0f5979f0c58e83504464b779f9483b3942e88c877f370c0a44a13e66e5ea9b948aa3876b2e5ccda712db215c6234447d9f8c07b7edaf90bf2b64" },
                { "he", "0b873e2c8315009a54e26073b082231b710bf041d26e7a9cfbceea1f345c030f910db11d8e3630e9455a8c9629c27fc2aa1933de178935c80285c68b5a3a6241" },
                { "hi-IN", "f2098b676df8020b1d1c479b79bc1b00e064dd14d17e14fc556139f24e88166d0e47f2f98c95d4e8dc4f500ad5016c049ee510b412502a23dd7db81136bcfe9a" },
                { "hr", "292383ff616c67b3f4229aef0bbe1b96d102ea97339f702fde8608faf5eab4f4bda1b8b0976d36d44db8ce6d7aaaab6cde4e725295ed8fcf12a80e9eadaccd1b" },
                { "hsb", "47f07f75067cb9fb351b58b6c75bb48c623783c07d63827805e56a945a080c531fae4954a6a5a064e6d6edef257a330c8404199ebfc54c981b68cc3d35ccad04" },
                { "hu", "ce27ea73f0d8b745112b1ecbf595bde0bdc3113226e1bd15c58af85656250ca5eaadfdd34df7cac25c9523f25d89ef05e6eb6a330a26252270d097c28a3b26d6" },
                { "hy-AM", "6ec78b4e2fb63532115af894839ef394566ce970caf0c7de09b432f8d8b478e2d8727a9c59b4d150f5dcfda7028f68a1b5f9e64520924c8de9ef2f32f60c8253" },
                { "ia", "143c54ab2b27e2cc820edc88a52cba5cb9326796c4e2e78c1a89d25384153bd88c9aa1325480d85096ddfc26ba35be6761e9dc4e0048e17c1b8e7fe6d398aa2d" },
                { "id", "f77ecbdb4c3fe2cda33c87531f456f3c4e076aadd55bfffc41d4cdf3bf5542c99f86d0fbef78f7d50a0d1d87221eb191015e7a270881346912a90df46f4d0604" },
                { "is", "c76e2978275dfaa92708bdbc0f1a61888769a8d3110db21cac71db41c6023c4616b532d5f9ef3badfbff895076389a1eb91bb2ba558c5f87ae17ac231c53593f" },
                { "it", "453ee0691de0908e8c6c74906c6d0e1e2647e1719cb831aca1965f249ceb7faf5b2aa57bc6ff07e5fe788aaf1d879870bedb9a896ede4345cc15a8ec5b8274e6" },
                { "ja", "f67ae7c57e64df89547c4907d1c7dbedfb44153e8892d800c63254d9a9f77a47622082f2a204e983f88fc04b95b470ddb9f23d27e4a1bd9b0ea2b0a7be1347e2" },
                { "ka", "ba39c3de3865cbc2b96b829bf17a81d4edd4843d8dcf484d460fddd38f99dac683e9ea2dc46d1e393454341fc335c6fde80a4f87fc27a5571f77e82f7ec4cdb7" },
                { "kab", "403113475c99c78c776d1ce0bc72197d84b2ff0a69722331fe546bfaf8168daf690d61d1b5467c163c839e1a40ca1b1d044eafe911252e304ab5ab2f23605d16" },
                { "kk", "c8d0eee8b96e5011d55b11fcb539f9dd1e0756ea0f508c879b195d439746a9fdd5a79ddc81dd046d55b54d5f22da552d68fcc1cd18275ac0aa2ac300f83f9ebb" },
                { "km", "34438aaf1baecc0c3d70a9aa033063688e9531695738655d7c7b15fd99e5fe9250e1b582898fef78dec57c5b2ce45d39002b96f7089b256166ab4e2305dfa6dc" },
                { "kn", "321d8fb91f0cc347f47468933ddf027f47f9bebdfe7b4ff0cbbff7c8baecc53b233657fc8d742bac3822eaf90a291cee304fd2ccbcbb6099eb4d04c457f7fe05" },
                { "ko", "4b6d304da64c33594ede93bfd8e07d61a1d6b31c6863f75d31ad0a2e382c2dcb60eb778a7ebef8895be444feabb0e7cb39028fc491cc69f73f3d694e9948793b" },
                { "lij", "8435f1009ca28b8ac0b5c53e5231a7760b485f58eae872153a95cf65fdf8bf595689f69fd616fa260beff3b32f9ee7910331fd7592a9746757da0ec8e6c02fab" },
                { "lt", "6e8944f7e2b91c40f5981675e6d0fcc935bc7447f45b24fa2f657ed440abc380feb0e068cd790103fb54a30612abe07fb86ec610912848f20a941a4bc0c6f05b" },
                { "lv", "2789cab8fe5646038be5cf41dd151c391763e1add09f80158eb1264b6852078af317bb6528ab9c4a699cd899ce31a987bec90e48ca034942507bbe693ffe156a" },
                { "mk", "3e5e866fc294b4e4baaf32c05053b9fbd4cf8cbcdee0b4ae479709f003fc46631a1346359ef2c7f28e3e05ff5234aa1b0371a90c411b11a57675e5e52eef964b" },
                { "mr", "d26a327ea26e2f62e82cf1a6033672285a49fc06f995d6cd9d03cc6f250db638661298186d10ded44a76510675ba8184aa65f825fc996af3543a15708d21c978" },
                { "ms", "dee9982436dc3e7c54db68cc1602a5fd5aa3c6d48e9bf14e943da0ec277c0bf4b207a57ae804dfde841281b351e220f8515e0f869a59e72fc8e460891b36e2f0" },
                { "my", "09a77e19e0e8435f8e068f05220e2d952511df8c058de4514c6c364ecd80d45b7ceb819b118842363677e683db05a48624021f45a63654646eb6158bcf8a9191" },
                { "nb-NO", "99687c83836534cf7b21951d6bfec402b9e3af6570368cef1ecc40eeec295507ffe4b60579227718044a397297723fc69b75814d02d96475766bb9084b33fc6c" },
                { "ne-NP", "3532aee768ea3b843b90a94ed3b17b14f7d184c2ad77d381eff43d1b83ce54d2f43b7961e1d1f2af36c51cad4eea78e3aef755905da3607b3e5039407f3619ef" },
                { "nl", "65e757850db345f1f3e030419e083400d248527470c14bdb067aa96984bcb562371d03abfe24e832abb7e0842ec34dd8cdf2abbf6a55d9a52cffbbc22c6b250b" },
                { "nn-NO", "9753dafebbbe4c9fbabaff675e4f4fafd9c1a3348cbffc8e581136fb5e8d87ceae42a787c31b1c33b3f47f7d137a18dc6a5bb344ac7ed677804f12530022ac0b" },
                { "oc", "40ab9ef2ae72cd1b7cf092725642755edc048d31ea61ef686171a2b08554c243deed067a7822bc05bc5b2db8aa1ed2db26075ce29f1b9ca8b0db654654d695a4" },
                { "pa-IN", "b796452621a2fa6ccdfef3036d0775d9c72b7e5cfd424e9de39df8e39f05eda4ec755c2358d994f7aa89653049d0745ca418b50bab063974502783bb53ad1335" },
                { "pl", "45c35b2ba123c82059b8177205b55ff6bcf5b97e58a16e846b0ef601dd870eb22c887a8a22a2ca406dcb7e019bdde5fe176bccc21cb2b511bd18b0760a6c43ce" },
                { "pt-BR", "9293a15591a183c2e62b694d665083134f6b425dee04f0172007ab51078bea67a8cb164b870b89f2f3520a99a0da6389d83915582281dc6c2c87013bc6addeb7" },
                { "pt-PT", "b781f39df0668c0a6b7bb447e078131b3cee5077513e9e4730c642610ff403593116d9d25c7a8ddaad988528ea16561a96b4c761eec4ce0020da3b9318ab3cf3" },
                { "rm", "0e33ec5645b849b0f11050421893440244ba8a34001113348859308459273ea382111a3ee0381960a87a236c820974f2ca1bb6753a1a169c14cd89e58e48d9ab" },
                { "ro", "1d863ee647b8e48908c4157684b32ea564087c38a413bf222b22ce4ede13f7e63caf2c1699b7f774ef6757640375433dee0d6bc72c547680f14c80e60c3b132a" },
                { "ru", "622fa2d2702ef8e499242fe493f5a3006a99ab07ffe113c221152c6d38117107b2e252172b95ad6c8dc0a0988be6e5e3eccdd8db56cbc1b22d1d2442c501e18a" },
                { "sat", "290b26aae4b543bea514d7baf0016f5c376b631918d175cdd79b24ccb6bd914de8b89cbd4a8eab590397b930ee29144bb6f4b040a6a49880d48eb5cb867bb6f5" },
                { "sc", "9095562904b0c411d5d8bd950238eb11e6b26572a02921a9bb66cc6d0ea7aec1183557734e7a16f724a143043bb780db939b038bee958ba4da54c705caf2dff7" },
                { "sco", "93acaade54c4ae809ffbe6f344ca75eb59881e6029f460fb7c3caa0c2c2722c5722c0e21e8c4119b7c55741804e2ccf8349f1d7cfbb2882da5139e25be915a3f" },
                { "si", "c2a4e18eae072cf20efa086e47472e08679722ef6b19fcd9ae8a78096024143a1469c3f57e7c119c6096a152de2d979e9e426753454373a50543a288b7f3abc8" },
                { "sk", "eb43172a26c61fdbefe87eb508498fc36eb0ded73cc5a6aea2ea17220c6036f515a1635d132b12a88981377a53c2ce33947b965a9f128f0a4d677c88e20043c1" },
                { "skr", "993da0d46b39408e89b639721446e4cede0281419cdbee0ee704230f47a01463cb4ee366f55d4c1a835e89ab1b6e84a4ae317171e8726df5d52bb550fe27ba90" },
                { "sl", "4e0541ca96fbd7664211c0dbee5ab89959651ad8160e7a08d4e9bb859b4d58d6fad2c9a145b884d9e3263465ecad7bb1e51e8002537837e832bcab27a9e5fbf6" },
                { "son", "f0e3bc6572bff6a9b80d673aa6cad1c7acad9d6cd6697c1511ef8dd4d6e92010a79a1e51c2bae53dd754d44659b9e146e7b1d3925d0e5b6332b2ce26f88d0379" },
                { "sq", "80a90b8e27b6c98ef07d8e2b9ceb1861ec8c2833da566d2eeb8c628ae23fbe1940f77275fa71b14c93976ad61cba0490446e991b30c82022621530b83acdf08b" },
                { "sr", "0e95b5529ade629fa3cde93d7e70a7f2ac5cc2782d941c62dd7070ce26a72ae2a96fd903288d5f4ddcc0f24f7a67e6b951129ad56f9c5ff50888dfeb49698456" },
                { "sv-SE", "d91a2e6dac423a030a81086d3786c7c75ec0167a136a84393cd1958cae3a07d0115973303fed9c904f68a8826513792899ee1347e1f92e01e342b03a1f018c0c" },
                { "szl", "a909f9c683535ebebce72a336c83ae148b7d6bddd03a5b873afdc26867d7a9049aa9dfbbbefbeebfadf4f8e03bf26252fe5bb70eda71706cf2f68d14140393f2" },
                { "ta", "a07571b15c4b6317e4c2d491f118080f0f908c0a4b7c4577dd18eaf1241a6af42f1802e1f850f8cbb4f121fdf65d3e19aa1ac9f5f489dd9489a86aef5d726ba5" },
                { "te", "6fe5f159825f11275030211c6b92aaa6298a07d95c115dc243e80eb1af43c0affa8e6c30bcd2812821a37dc3176ced7484b9ef6a79f7fc8a45ba921808f86e1b" },
                { "tg", "be7de7ca24d9ef00f5d8d913ce17fe882af8555e73c3341dd52367caebc9ac7536f3e268171aa1bd22d8248d9b6d85960008db899751df9369571d577f9ebe43" },
                { "th", "5ccc556b5d8f4e901a2db4d504e5fc8f9744dccb09bbd52d69d75aef307c9653f5bfa5bfec6074674ac7c542ae876a52691adc878a4c422d38bd095215c82196" },
                { "tl", "cdb92aed4aad6e8350c3d47b924118d591851ed3e519f4c89fbf7d12b0c6332ed8298de3989c2eff13e71bcb0243f471a2b785656d2957594f7d94990c32c5ef" },
                { "tr", "ccdecf376ccbff38ca9b7904697a5b3b76a46e862923309755ca92b7788a99b2abea1be373b21d9cfe878aaf005049e5b3d354ed29c0432470abe93eecf9b18c" },
                { "trs", "9b6a2c25c462c0b684bd3a11c12eef6ef219f196502cf24a28c01fcb621662a5770360095fd199d80945d37820b6da8e0d367c86589b8e41c2b28c8b693f1da9" },
                { "uk", "b141472746831fd322fed2c11f93a1e6eb7bef900be7b21e4af96ced3787765c1bb93f121c4153e5df3a339eca70e0a4bc8102c54409b46aaa22e56dfb2901e6" },
                { "ur", "eb0bfdec69ea378e27dede91fa1dbca771903494f87e1702e87252d7a61cf28b3c3cfb039f5e779a393fc998baa69df6e56aa5e52652f369f2c7355fd61d3174" },
                { "uz", "fd34087bb1d1ff2b426659ceaa635ec04b2889c1005573d017350f441dd6917a6c0dfdbdd3fe61e74f576be8c762477a0ba1762e7997cdede5b57489d5f0c45a" },
                { "vi", "41a0eb7cbeb54ef552ce630b627199c42a93a17fe4000ddd3cdfe6c9c8d054ff38395fa7b1e219905ef28ce4b39c492cb14f43e269a94fca27ccc65981fc8091" },
                { "xh", "0eb22d9357722bd3f8e0014b2526ff1bf61a4557229ee6cd390e0fc1b4dfffeed92011f7ae4f8ae053f2eeaa539bf625a1fa30cdfc353ee9c468cd0ff7804d95" },
                { "zh-CN", "6b1536d328af12229f761a818c9e762e4e9f8c505e5404d3ca8b0d5846dc3e77d62b9e17f03a5a2b7a3471843cffd1ae46e5c3893990cf201d7da470aecbea3b" },
                { "zh-TW", "f74237d42a7bd5cbceb5875382bebbaddce2e430bca63047b71dd074c710769d9db79ce4d9710c5263ea4a7035056e80a8ebdcb01113026f0f1a125027afdf30" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
