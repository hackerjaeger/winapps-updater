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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.10.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "3116306cca6bd84bc782dee394510eb29bcf6d2d47bfd01ec89f1c6a6615e0d1e6bb5d4eb6ae961b7b840da787b648c35d3ca158afe39ec2f1c005860d4a868d" },
                { "ar", "2e2736619ff678b6417ce2421f2df2719cec5313ac6ad0bda367da33795b7372d750cd5d7b2d2dced8d26d044fec1a76aee4364af97a7c26c2558f89f1724558" },
                { "ast", "2bc74daf5c0bf495b955d32e5c08fae46e2267f4d59abc8fd2a02bda94eeee5cfa145ef6c471bd631e6c7d76a9cab4aa033176683a24f21624c9ab4d10bef824" },
                { "be", "3d9455305b749a162eaac20b9d8b7a146ff07d15a0ff8c1e55b58f5013ffd6b2fef589f5fc33663f7c1353290bcd8fd1e7af5709cfe2020c9d744f49628d237a" },
                { "bg", "2c66d96b276dd70d1ac3b8e4076d4b41bc0af01b9a798962e61e2ee310ad7f4499f22b9be07b408a01ff461e889df6c0bf3c7d95b5b343808ca6e030c8c25694" },
                { "br", "fd566856c54a4762ac2fc9b90238a898a07f6f168988cff080d419d6956482e2f53b0f239d5a01121639037d3a09a4d960b4ac4e098f440e32611884ee6cd305" },
                { "ca", "f9b3d0335290d3db5f5fe7dbefe8c4bbb9fd6c89ad4f94b339f63a448ba304079b7099bfc48b2e727575c49a5e5bedb087f773819b1a14516b2b8bed38aa2ad1" },
                { "cak", "a64a996a3f7bc591d8c6300dfc1bbfc098f2babfbc8706027db73af4f0850a85f481c109be25167f28f5984eb10cb105453b9d7fcac4acb97e370633ca0e2830" },
                { "cs", "9587d70abe7a7f465ba812449912e9015d6c7558a88a57941cc7f47f53e6990a65c5cf6b896d2ebce9b63e3ac1a0a73840d2bc701c6499aa8fa5a52220f710b2" },
                { "cy", "607ec01a3d90b3a53e188a4c69dbf5585d4313862de2f55c77b5aab427e443e93238e28880c017f952005e08255745a480348dba54bf5d59534590b49dbea41b" },
                { "da", "f665a248b39d81e3816031b1ccc5f5b7406f031150397c907b27d3181813a42bf8f0f280b5e1111ca928bae1057ac9a43a5da178b4426eee3cd398b82245ccd8" },
                { "de", "618715c59e220730cfc5f15061b4e5f695024d5d31906933eb648474b5a3f238abd70f9794763ef75392aae58b56c05a1929b893bb5cc9383f7286ca6c98280e" },
                { "dsb", "104241cdc9fe497145b2669318e26213d8b1b63a45f33aac5d503212a9333b1d8aeaa22bdb2268e6fc1529bc7f0d2f9d6a97759fae60f0840ee7345c4c1aab2d" },
                { "el", "1cd4cf73fa131e5b3982fdb3bdebefda860124e7333e6939c778352cb2bededc3cc64d120993432b29bff2f7e22e207aca16578a0a31ee5f304bd2ab719b9196" },
                { "en-CA", "1371c4d7b6821e695c9dd860081e9d4db739958040b9e448f5db7a2efca1fce6b39b67764de7e0250362756f66a8ab4aaea571f2de31062588e487219bcc5518" },
                { "en-GB", "b7fcdf8fad4111cd8052001f437a6ae5aecf7eade4af2185b7640b0f1ba417079afe2e741e3d2e46d46dd09811d509f7b60373bdb84ea0586954fbb6e992f1bb" },
                { "en-US", "c7b8d25c6ba5b41b82520da57af23f0d3fe9d2d1c80b59a0a3dd7a65d16b859049702183bcb13f55be881f4f9fb3779a85dcc14803abcd6a9c81c081498d7b08" },
                { "es-AR", "285a0c843a65b6da16b4c47cefa65fa86447b4d4c5e05e4b5ab960810ec7be3ce704d62a382b15d7268bebac58c3ff46c8ebe1ae6e7f1b1b1bf574e3e5834b6d" },
                { "es-ES", "268d9087b5721a6ac340cb4e1536865d0c51fbb307dfd9d07a02cefbe2031038e84d5e68a44a7adff9d6ed7b3007b01b30a85d1456c81f6e7240d18cc262e84f" },
                { "es-MX", "0f493b4254a0c488f3b40bb5bdd6a8b756376c0392d1ef4dcac7de3046669acea629d5f47ea99e9458ca07136192b8ae79474d3dec0326465b60cdbdca774241" },
                { "et", "25051b544ea3f1f22a5276e9f294fe41296998717bb41639633f5ef8b98b215e3666dfaa64725313f703fa0933adf61ac2b818b6f28c1ad084c5d2444d056cbd" },
                { "eu", "ec6207d7968ef5ddbc709d1092dc7e941eb4d8c3dac4d0766e99fc4cb91a39fdbc653fea3d7ad2e48256b6bd3e37ddff7d68c7c4ef43774fa3dc6d3eb626617d" },
                { "fi", "6b35496b5edb24dfe82b08161e3156e6029e1e16b796f84c4ff4bb1e25a6183d45dc3d801f75cd04d4f5b02050f5eced304c66f39d76236b58a3969f76e05b61" },
                { "fr", "a3ee87f617a2b87bde2065d7e8d317f28d553cd6e42ed5140cf2438fae4c4c3ef7a26436540e46e9f8ece0410f2610a2eb5eee05158ec71dd14c2e488050cd4c" },
                { "fy-NL", "e4f2fc954e4efd7ba96342ddfb5ee144e562ea0f2d114c5981748e91174461f8d8e65e6401f24b4ff31dce56036fc600fa970106c56058d5e8be2ec5dc3fa14b" },
                { "ga-IE", "abc709294caf0f7f6eea884831e4defe660674294970a94cc73f1c65d78a511daa28698afa9733c0b7c7bac8794f363229cd8ded298a6927f4147dd7cfc7c937" },
                { "gd", "550198ab1390fba2442c842e8372f1d8682cdd54545889290030909866efb1b8644a80b833e7e1aac1393c8482bb0d023a51d042d6df941b8195b76db3534c88" },
                { "gl", "5f4b9fb5c6f55c31494e0c8a9bd811777a4dd7d5efd87119ad503223059ef34273fd64da87c1072ffe93784f215e08cc937c6f0940e036911c06378ab4e460f1" },
                { "he", "314a2218f44636dee0b2870fdf47380fa24f19a52884ae8f852423590ab0a6c41bc8e609677b790619f5f601074551af8336008c45ceac3c715e8036bd10c00d" },
                { "hr", "073af8cab624fdeb6a2e3a109f1ae067210681b4264af4cba03c8f38cad75a3d44594f9e09fc5e44c46868f42b482ad2ba8d17818d3d48aa9db4d333f2dcac9c" },
                { "hsb", "2212b801380ccbc25e4f631fefe83056ac8f4e33b2ad4e70a0f7f2670f9cdb1af8698c791f5876b159daf11ab8e9264aa55833b8088a040668ec4fab253a1d69" },
                { "hu", "13994c5dcc7d358cc5a25f55a6358f72897419ab5e5f3ba79a7cf999c9e5a2c09baf31cf2eb4f5623c62a707acac4d234e47f0738d31b8b40b92cab57730afdc" },
                { "hy-AM", "2632f2e3f4380bf0d101ab6a39dfd5e1267520d887592065c4bed2c5bea6602ee41898ec87dc38c870020ce39a38daae1d775cd8ccfb5de44c8bd08f4b31d962" },
                { "id", "85a8c213d2a3d103166d49dbb623c026ca79980b01cebd9cca2a88e44e8e63324d44b3b846a94a513ce673d6e3a3c0889ea4ccc2ffad4d65b186359baf29c3ec" },
                { "is", "3ae4a848ecd7dc623a3ef20a81dc55a4c7eca352511de59af13cbe88ebe0efe835f310354f077566a8a841159ef9719f622a8c79d5f8964157e9ba8679d19e32" },
                { "it", "e26410f7d2b7eadbee21d5d0e1c131bbc09d3c074dc22ab4a22da7121ccc81a81cb21fbcc5e5c7747b1d59b5eac3c193c7a64f693176691906a6fd8376eb5f3c" },
                { "ja", "430764f42393a5bd6ad49d00644081057b77947499629653a5fbed8d8b208f16094716fbd4116c16f111941cfa2fa99ed1e9c971bdfb1ea10ebb055569d92ad0" },
                { "ka", "53553ccaba5ed637dec6943df0633142eb7b420edcc67b031263ab2ac082479df0a42cdc05612cfbd95c529fd8bb9e2788c968f4c87af1c037b12ffc767920f4" },
                { "kab", "2b0148f2732e342a67b297b4bd780ba2e1ca80d91efbc2dca5bad81d80bef898010b4c7adfd12e7e01e678ca53e421f1229ab1240adebea66d27fec59f9f2d8f" },
                { "kk", "07cadf086ee8e97280eb10d07e7d716abb88a8196e6ea8022a49acc51ac08c7bc4e24ce0352f9a5d0cafa42a0d3487fdf7221c8a6082913ec84c1daa55088769" },
                { "ko", "e0617955aedc5a5500777b25c92ebf69072603e7a97be0c9e01678a824a26bdb5b68ee03e9f5a799ae027382f3a902ce45936f7551552d99021b4993cbd9ee1f" },
                { "lt", "180cee532379e7d638749297c7c0a9a1cfcb3fa40300c3d3fccf4aec3dd8eccc6e1b3adb34b13a387e3abacefbca04fc307700248c1c17bbd121ceb2dcdf2de2" },
                { "lv", "de3be42556c379fa3648881a8f58ddb676d8e2bd3ae3b23e10644fe9e9452e14f8f5ae6d19f407bf7f6a34ed09413d8cf43882034e0e3c68efd5d4a8759b1441" },
                { "ms", "c31f9744b6e9c77cdeeab7dc6c604159f5c65d8597982dcfdb4c472b499ccfce695587633a05dd55dbdaa4170152bfe50d4ceb2f0917a65ff322264d920f8594" },
                { "nb-NO", "ad4b17ec482edc8504d2c5a37f80de10fe4b5fa7b0f8ab81012ed876c904472567868a2717a5bfc4206ada9dc8407ca2225f40dbb3f532fa5717a958a0213bdd" },
                { "nl", "b93c51cdf0b2e67f9b079b285d245c146f81b1f4a80d6f978749652a860fe4f22c15cdca9c4da9f116ea4362a9c279d9df5978df00dfff707f449369d9e0e7c8" },
                { "nn-NO", "9cb1d7e5c371397bfc89194bda885e623cd4947a542e3c8e294ab17241205fa0e6e5e3247dd5266b07eedde5af57da88134cdcc27eef203dce4f35d971ad6cad" },
                { "pa-IN", "6bdb3d33cea13e4bd1bd2df1abb4f2e84fcb1ffc5b748a009e71ccbc03af24b3c4faa4ac93eb2c6fba3d9077905b38cf02d97ba85b38bc6be1e3c64708dd9fdb" },
                { "pl", "6429a5a24dec2821d5f3c41aa7b5181a77358b2c3a243a57ce4a201784a025478be2dca0f40dcd4548d8c3477aa5c27ae3c01e07c5a05f112bcf84dedd6e7668" },
                { "pt-BR", "e048c3c5ee6c6800e5c41bd6e534608b7ae1b02faf48ec154b2ce1cd552f7312c54445133213ca93b398addc39e64ee9a8763c4fd0bcc60b060633b34fb7a3b8" },
                { "pt-PT", "cee1558d451b1007215f0e6cd92967d77bf749f5267dea60fbb8e448b35cae1a2f1744c9b2a73129bb9cff0224ea660e3c7e29d264c3b8e152d955b59b2cde1c" },
                { "rm", "050e4d276ebff19b60baf80f2ceca86c500f64a9ac5bffdbdd9817fcb5b1546e4342c517ee6f6f6bab5b11ebb7eb20dc2f8279b25086f8c3063302ea6e17933e" },
                { "ro", "949a122239c54a73454d19c8ad52f2a77e0ed6fa8e49662d6dfcc401c1b73b29b1ceef2e769c755e6a628c7fd92a3b7f23acf8fb78be145e182bb15835c523eb" },
                { "ru", "2af7bc33b1ce4debd808df89bdc6646aa45183669b9fa52d3549470fc6f185ece4ecc1b8188565d784d4d2892938d7094c60896e8d1f35100eca3e26cd02993c" },
                { "sk", "61e53a279a86be874dc23c23d0919c9a9041640209e4185030318b06bc197a830f3315fd615300b44504369bb5c3797ab9c954b01131705645469d5988952218" },
                { "sl", "e4fa8bfd69f8b1628bdd52aa9cb1c312a43c9920531323149878bc79d3bc213f0cd462cc8bf4f323b8d3fef71e3564b89c577ae903f0be1d44ccbbe60872a40a" },
                { "sq", "b43f4c4c37e069ca80c8a7f8fd507f620119364b9faf1d0b15405cb7f4c17235f4a6e1f7b1ab7317ebf1eba418f348409d7087290c2d8553ae5d1fe0326bf7f7" },
                { "sr", "a44a7d43ef3aab698cb3d2a7ffb1e528f44c41dade602b930593bab41b1c1ed9cedf4abf0045b32d97dc93a1e057e22716e27b5c5baa772ef83f7f6d2ee0f6e8" },
                { "sv-SE", "2db40a81f53b95ef653a88fa26b042e18097d110123b1739b2d2419c2352b59a0b5c8e1abfddd3d4078eb08cbec0053d53e6bcbdae87e35b0267e2c7595d2a5e" },
                { "th", "5aafb84a4ff6dcf571ffbe11b51eda14460a0754d92f2f6ced57e2e7120d120ac59111a1803f5eba9ce61a0c33f43757486dca0b27b7bc4564cc6ee88cfb9846" },
                { "tr", "5004a52022b25692ac62065d5ec27e4c7793da584aad1dd9ae600dff4c414101def6a629acbee8a2d394ed6c150fe97db5b7412217d0695c2ee7ca99d056b482" },
                { "uk", "400a060b554ef4fd943b81cf7c64f863b79b3956c5bb4ae688531fe54bb4bc00099f1c6636af896024f9dfc4ce2c6475b02f0ab841a2fe54b5f41270150aa692" },
                { "uz", "912d0408f552ed0cff9f924033e855b8c044e813f9855c151ee2c699a0c14584291011a6be3a757a5387677a72288590cdf2e86f18badf8a26e66d17fff1546f" },
                { "vi", "55afc1cf8fc614c7ae7ec680ac6030b5dd3b69026bba99ac8a1b1218b33a0b01d590ac6f67b03867c539e3c1a681ce2f06ba1da9a5b9b78d0d222502e7f2f0d5" },
                { "zh-CN", "f6a7c1a31721fa047e3f22e4abeca7cf03bbabf6c34fdce62d42807f923f3d3aee34afa46222f651a827cebc31f0ee440fc08cbebc3d44cd45febe91d7313f3b" },
                { "zh-TW", "c60b1b44b5d075b512853613c81243801e4f47714ebdac9c31d0778debb0dac42af753019463b680f85d1ae854723f7cb50de9190b76f77882cb2809939b1e5d" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.10.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "16a3367c1ff3532432d9587482b348f5c0280317b513a526873558c30f24b2ba64f8c3d604cfb28052c86adfb2190e25811271ba164482fba2b69451448e63dd" },
                { "ar", "b4d1f8d6b8a3322f06d2c086a2f798be74e90587bc8e7ef2386929ee68c385c742b6e234722f20f04c4a6ca9239e435dea94cccdbe8d86eed2b6aa446e3f7a99" },
                { "ast", "6d7c623fbe54ad8bde8bde03ba29c50949588cb1537f7cdcf7942f69843a94824469075083c02a471cb3eb0933d7f3b2e5a1c840e056eaea7d99bd545fb5e8ec" },
                { "be", "5fce15fc41ca3f7e159ddced9eb3bd663894546a560b230c07b4d0169d14f1619dfdde75e51db15d5b718ce2c55a5e6e1b8e29c52aee4ba3ecb13c8b1dd094b2" },
                { "bg", "790e4c5203dbb7139487605bdb803d8dd94068930e690899817e67604be0ff1ee12c5acf40089ee3ca4843edf939ecb62e97bcc2ffc016c71d7989cb38ba2457" },
                { "br", "982ff43063930ba9517379470deb39b187fd9db19db564fd096e924f3d8818c48d8ac899be6fb637558884814a1e9b59c77d8defe99d11ef151e4ee996bd614c" },
                { "ca", "e3c4882b6ee214cd9fb21d7ac366a803090a67bd3458d46b245f336bc4c478bd94555526f2ba7a427dad2520f6479941f6116b189b4685f60ee9764f0f62248a" },
                { "cak", "f0eabb53b6e1bf8979894a5560ad93d31d31c1d163e44815de24c47fb9eff1a69a4a570491cd692155c4a4a077bbfb6c1a4cecb03314cc6a7c0f58f6d6fa18d4" },
                { "cs", "748528c3c4ad6fadf8fa6cb4a8d2370db80c0d722c4c5d9b2dda1aa25f1cb2e7e67cf95a34f6b07141066d4e66e1c3b7679953abeb584948946e86b61d829d24" },
                { "cy", "81815d6a34828d8611937c52a6ac3551316653ec4596e3f507359e715186669616bad679bd4b5916d47f8cc0162a51c4a215d2159af440eba29571376cf28ee4" },
                { "da", "93b0ee2e0c12b702e3d2574430bd6b755ac2e957a4c5f88a2e7af7de1ccc791547fd6669869b3019c77408a4f1a5665767ded6a7157b104a2c81d38b050b78a2" },
                { "de", "b5cbd27b269a734206d88b1ea020fc1c78db4347de50e2ffdfb797417b315cc53803f22e37101282d2b1f21f1234cc8a8aa9ca2c2751a8f48904dd1db6c8a6f2" },
                { "dsb", "bdec2a8cbfbead44e12827d002a3041f50e37af4165c2517c1326f172f9884f99bf7e4b2df679c87c4a57371708dea48f2c4723b3cde7d22ddb29cf7c1353d66" },
                { "el", "693f867b7cc02172b1779e59045350f0d2e3f851a051366501ce522f19f5842df0194907a658cf491f23357cd6dfeccf118e537a0e09b8911b4351d124e40bc7" },
                { "en-CA", "fa48816727b1ff7b7599601932a3b492419e2e944722b128d2aefb96fbdfb73a364258f52ef0b13c3e7f8591f1f1113ef4b45feab831c386a7b41fb7ca37ed4d" },
                { "en-GB", "47b2d50f58ec28159066133eecbe30b38350092b37733957b4bd634239cec123dbe9c78b32069748fd3f0dcde213591e43bde202321afc96a98a32ca3ce23260" },
                { "en-US", "5836a09379b1d9317321e38d7c40f4d158ff8b6594ed4b66bb685f49a0c8871b574028474fea2e99d89870facec50b45624e5dc71bbb8f8b89413571504ba4c4" },
                { "es-AR", "834b92a357d8d2ae0e55644dfa5c5c32dd10e7d178984376238f8f7d6e6b3dbb334613a8e0aa63124080ea38b9a15712dc64814794dccc09ba71b215ebda7dc4" },
                { "es-ES", "7e367e34f6723d3095275864f2425a9fd25a8600c7c64293cdf3ee0f327e2de646f8c669f941fc441c7639854f32920c4a5766c71203b3f1c9e1fe14a30ac939" },
                { "es-MX", "0d8a549c74e464418011f61b18539221cba1abeda27ec3fb2ddea64edf53fc8520e4a0cd6215a0e5d4eaaab78f03b4f424f25cd31561277071ef250bbc3a75f6" },
                { "et", "b90ea08b410aef648492e8fc851cb94161f72ac52968f67f5f7e031ab29f6038f24d652bfdfc698976ebb447bb4ed1538a36b9e802009a8032b609bebd5fbeac" },
                { "eu", "b579d6dae2c19e708c2fb9bd3b1d06fd7e53842c9a7b47326a2526cefe59deec00f76d268d52d33b9e04e9ffa15dcbd877d58b8a6ac1178df24720d93fea7fda" },
                { "fi", "778f57da63434aa5d84d9ed41a8b6922c42868d71b2d8956e795684fcfcd585039b1a88a2da2902d83c1138338cb538949ca1927b26aec744a7a8b8e76c93817" },
                { "fr", "ce43ea64fb8e253b0328d57f8c2c43c22cc03a9f59bf60464d9c4a25ebf17cf9d6fb438482dce6ad1c4bb4b8282671558d5acfed6aea695c511d0513b2969f71" },
                { "fy-NL", "a1ba9a47c3e06d43c428d9d0553d97439bcc39a71b30de9f0213b130f6e8f45e1da8fe6d016c56714cd3ac07afe1d7d7395d0cf8c5b42727a0392b3ad7e6d78c" },
                { "ga-IE", "b00dcaf2d308ac0ece074c30bb0b143b97a828a5dbfcdc4648468e5040bc00d79bd47480e56edc5e521a054aee94f62ae96df6c8fd613f6f859bff72840794e1" },
                { "gd", "3709f5899cfddbade02bb4a6d4b054a9fd2fd961dc208346bf81b6c8d5ee5f4605b155f65b000d798c3903d25b26a40ad66b60d4c7150bd4c17d7273d736d1b0" },
                { "gl", "9d169f83a886faeeb89f695daed9a3e6f68e29be390085c7435def69375a94e605b85a8fd53ff24b7621e8d5fa97ac863abd9d85fbc37d723a099be271978de8" },
                { "he", "d95d16f6b7aa229ed3f9fbacaac3a9933b63b4b02ef6b86c08cd2399decf95fc9aa4e1e6ef2afbb4236e57c5608984efbed514ce1775550adf681925c57c910a" },
                { "hr", "6e0247b19495604ce2d2003e69b1dd3252d36fde9214fcb2892ab6827d11b668db79d65d3c5e8658b51903267ddc773bcb46c1942e4c640bf5dfd0186d50fc22" },
                { "hsb", "7d31ddc23a031cde43a367208a903b070caaaa1a47cc24c35ce4fbb4ca57ef273b4412ea3bade6aaa19458108fd0df3ac219cf802fd4bb01c96a3ba5b53f423e" },
                { "hu", "0331f92686a0f2f41e9ad897ae8ef427f0181c3e99b4f2a18c77af939e0d2df74772d6839df1ee7ecf829950a9bd8925e6ea7ba4338a7eba5a9de0dbf5e9a653" },
                { "hy-AM", "d94951312211a54141a4eeaa00a1082198f516a8720633e2e1041a838f81217c38ceacf83d622309d0311322fd5cbcfcf601fb8d5fbedaa3e7822299dee9f8c5" },
                { "id", "f2caec832f0cf67ecba6b5053f0c720e73924fee3d301b0addbc7022d5b8c40f48b52053ebb9a57aef24b3240047225107744a6519730bc5714c70c555b7047a" },
                { "is", "ee964ee444bb11dc589f9b8a1182701547c7c2a4f524dd796e06888ea030b92ddaa875618fcb7eea48511581ff384a22f11d6ce720d0b845805711c1ee10ad44" },
                { "it", "3bb8a7d434766d4382d853b80b2430c326565a3f5a571fae1821e1d9beddf6e448871bef98351ca7a9e72d097fd6f2b1f3d42d6feca26bce4c035bd942bb8c47" },
                { "ja", "0d65014b32e8ceedc2c625cdca9625483497462e4fd66aac5ba44dbb9ded597e22a134cde284e4eb4e51bcb1b30ae537a1f3b3c1a2ea2d0ebe9b685e861e1f55" },
                { "ka", "46570fb299d8c1bf0052c7cc8e947f11fc81bcf3a6bc5e6f4094ea0c8c458a97be3fb489c2e9c2eb31cfa40c329f533582d59e9c8cfe55542e1f5bc64b6d67ca" },
                { "kab", "6be45b9d969d230ae61025b04ece01c43a9561dd50139360d4e9e4d69d16dd1317e92461625db7a4844278c8a435ad74b668c7399683e63df2d5f5df2e46c86a" },
                { "kk", "40581bfbaa0bab6bf34da31e8257afedd29f91c8192dbfa3a05a422d339db86cfdc79d416ab25e8ed2e026622cc73629cf80cfd545f71a4f19ca4df0d5ef2fa6" },
                { "ko", "025bf6bc91bf1053410a8c7578f238f2214eb4fe06678de79bd7a7a25e560875068dce34b9a78df05723725a66759f30c2ea99bcb0a71c6d2cc3f293c5ec6e8d" },
                { "lt", "c2c8efbcd51395461bf26170118447c4865cfeba7360d1f9d5f06ca4160b987877e2853df9320fbab4eba805d643bef0a0d97b81156f5859a05537de38132afd" },
                { "lv", "08d1c3867498aa3f12af65a3700f77c6577d99f6896bb5d697500cff056ead7857e5d7da35829ddea1380e5c2e43f41b1c10c89222e148fe43162ddb557905ea" },
                { "ms", "96fcab793ac475bb1737be6911313f4b228d7e1109434ccf252b915af5a28fe8dd103b5c4f197bdf492387a95d013b41a929d0b5d80b5c1dfcc441b9f7debc4a" },
                { "nb-NO", "c9b9826554ea0845ba8aa1fbf9b6ef4abc6631f2264e16f274fd4ec1eea48b730b9ffc54e7b70e59df2bf08b9a03bceb84569617baa612a874f8458fa161c4c0" },
                { "nl", "edd1189d079dc1b3a4a09f03b101fdf1171e9d50e2472ecbfbc102a6b24c30d9df74b46002242c9ff511af304188ed4b45b1985264e6e550e25fb3e20ff48640" },
                { "nn-NO", "3e16cdd644d447d20a909c9d4d3293f8ea48dfd21d64eeb193c4fc574662fa32cc31408b2f8ef6b4ea8067194b08187e40f9b2118aa9a037da29ad93da912660" },
                { "pa-IN", "d122717e5590e3b99d62fe976312aed4f8e295ebd8497ffde18ac4631e8473dea4ae0684a5c6efa001c724cb6b76a7c938e286238255b325182eb7576b5b34bb" },
                { "pl", "28bbc9c6086a0efc5aeb936cc50265de4d5157852b56f99991e16cdc2ee289c95b5c94417ecba76def2885c2b5a2ba12a6cced5a1c60fa08a9a515d419d47fb9" },
                { "pt-BR", "e403300e85df324108aed6e733627d2d98c328b48e7b24b65b35bde1a26dd4baec7b03c91c0d5ee6e0c824239193153cd353ebed9ecab7bf74222153287c53de" },
                { "pt-PT", "fd3bd989b2d86430617e023b1919e5dd7ce64162c6a321e10a3b97f54c36d765d45e407747dbe57704982066096b6e52b71b7570828273b7f497d0209eb0dfb1" },
                { "rm", "d6fa3075b3f8e393a71020f98792a7ac6b3649e0be58d45a1b52c4d2772e90a65fcf183a69955411254ab1ade095d46aa34073fc7a7c1f32a0cac7e7a0e89d2f" },
                { "ro", "b38cefbe2f7ad48b2713c107da2cdaacaa6aeabbdeaa0f2747f156feedea0c06c3b8296098627809feace51613d3e8a476039a27bb3298cc063731500aa7086b" },
                { "ru", "f2d69df5f35d76014f1ce80b31c8a7bf28bdd4ae324da32f3c45dbccd3e482bdd4529d6cc4cdffbd3822a90c2b7c5bb85a9c3ffa05691e75e1a866d55495da24" },
                { "sk", "3387939f134a65df53594a54db6a3bee9d8151e037baf5c43637ab4e5f431a25e673819d9aadb0ad69c43d6095f5350212e1c3f2a415b15ac5e2e6b80e357d8b" },
                { "sl", "8310fbf19aa8896c30f77eab81c89109f8818ce6fbc93d64ba4830a4cca0ade464a2bd364a2df1fb9838c864b2169f902b4328bd9ac98729c412a94edd178a95" },
                { "sq", "10c442d978e880ce7c9eb54d3355a91333964bb679290def1debc4778a5f214b6033f7c045e76805004e63203d1bcec2914aa3fea70869c8e0cbd4d9145d4375" },
                { "sr", "efb3bcaea4876ceafbd96d2e9aeff3eb0d5104a1272bbf366accf5077a44c24ca77d13630c3885409059b6d97167241c4d991c6c9dc881fe0f7e75b89fc72ddf" },
                { "sv-SE", "afc6cbb4f0ae88e295e7cdf09f005e702e47319b68a27c924d156e007dad9c92f377dc17a27b7d0558c8d3e71bedf3f67eaa4744cae2055b22a9a348b91bb0b0" },
                { "th", "3c3fa9434e5dbf4d7aad5df7f37cd1597b3b35d4983a6fe75203ef4a970b137ef8788e76b9b9d09d6f90abe08b9f2482b8dc78fd0d3cfd9da6c94c6cb750eea5" },
                { "tr", "93397fb4f40e136a202622ed68c70f68f4d612c45e07f45d05c6dc89ecaadeedcaac511d5b4bdbd8ada454f5793e761f751e477cb6572abdd1bbf03a2ae84786" },
                { "uk", "99e9377b693c960589b758246ab6f90200d3dfc40cc1145e80c7f75c8d39c253dbcb5f215f432aba64bb8dbe7dbb4f347025322a867d98c3773fd128412db74e" },
                { "uz", "5339c28bd74990b7ee4fa1bc8315d23431f1265044d155cf23448d91769babb4df80f9806ff8de9b8a316384894811a60af63ef9fdc7282ac0fe2ffe282856ae" },
                { "vi", "a3249a74d02406cd0075a32ef539a05de5180d78ca204892c8575d2c1d55324cb42b72b82e634e8a750cd84f19bedf283b21a1497fc96a9cd1dca1101c24992c" },
                { "zh-CN", "1a3bc2aac2557b5d6e9c4afc0c41ba0462ff13e6b79af798eb3e86056bb419ce04d6dd54940a96cbe91d576b298c97fdf72d58355cacf503d8e0e0af59d3171e" },
                { "zh-TW", "9f773aa6a8c5fc483ed8e5d39db25641da57d5ff58e696209a001bb736bda42fc2400ca61d41655f366c1eb509c130ab142bf8166861a2861b27c03695720998" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.10.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
