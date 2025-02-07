﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private const string currentVersion = "136.0b3";


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
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "6bc85df7759c12dde0061a10792a8ab6e1da82b575cbfd3bb6915dfaba3241b380f675ea41d1e0277596db325310e189a6c9758e80f99ade075a889d270ba54b" },
                { "af", "7dcdb3b105b1a70cb430d56f9c1ffe0d5e8ba402183282b4825716ab41f3792b3ff2dc68a20ab8df404a88d56430d774fbba8846a1ffc6092678c4ea8310f832" },
                { "an", "69a8017b2545f82e992eda862505834774a1e2a6d1f19344c9d74d1b93ee81636fcb8d9fab166ff98f825679942bdbaf8c9305b6d07e9ff3ffaa3706f7ef3f84" },
                { "ar", "beaa4af1857119b75c04df039e4df95f34c5008edca514b286722a809b6df8c224fb80da5605105689ac5b82be93ee2659e9d8602f0c4d1b4214e14f82c71d79" },
                { "ast", "452288668ba4dbfe358a6adc2b7c646d74ea80bd8093b2b985c55899b10963996f3b770d895de1cb0f44a20f0f4b1ae0f5665a7811defd7229248ee44d527917" },
                { "az", "81c7786e6419a8d4f5671098bac146d3b9b9e35fad06189c303e68937eacb682d931fb60e15bcd9e69913efde2807c7c57c799ac64a78a785d57ad59e807ea92" },
                { "be", "0724132ca64262ed07b93fbb10689791264e29837ca9b96151d69d33dd294533040657d31cbf9e6c39bf7458a74825743a97c05e3e243671d465bb6ef831d38d" },
                { "bg", "728e8de00ac00db5c269bff3c61fb7485d66de944d5c7df87d59ae8399056fd8ca301ea488fd13936f9d9d510240a394edcd64dc9aa86325b911049d6314f2c2" },
                { "bn", "995f6879f7cc85f7eae5eb6110e7d583e2efc3ca745c50b41d3d3ec35e7051f3423d61f6bbb12e0ca3525d7f63c0f93a7d325bf530bf6a1dbae8c9d24ee05f69" },
                { "br", "1db9b1e71eac23eaaa82fb2e92474e6653f2d79c981674dd41a4d7cac15c0e8106716ab81b13fd4b4645f78315c15a0065fbf03a12d8811baaa5a5c71a7c6f51" },
                { "bs", "0d50b416d147767a1f40f1ae4ac9bbb521c64b338329de56f1c1c177e4bca646034f443061829da1c255d7346103c5dc1eb4b13d62c98294fcc3c02d181c1355" },
                { "ca", "d4b10c88458232e11f60d8c8cef655de026539d9705a81f57c1aebc6c6df3aa5dbb7af0901f005cfaa66577adf964d4a55b1d14d2b1266a169da3ec9d47e7946" },
                { "cak", "ba9bc0bf8750db6dcd6c84d250c5fcb95d27f0f699f588d2f42a7e3c4a91d68931047dbe6ecd22bdd2bc65b7e64f893a7a8eb3da7431d939a0abb082a6c567a6" },
                { "cs", "75a64467a959e596035e08a489bfedf3bda6bd700248bb1365c07165d7a5b29a9c326ea376849bc34176d29050caa7352a22af968ae77fe140bbae5d0f09a72f" },
                { "cy", "1b4393856476f69d9b634cff4ab145eaf4aae9629e9ce278010a39388e3fb48f9fd0a02b1b2ac1330ea286f5323d0653218e2e81f913fb0949186b45c25d0657" },
                { "da", "4b1577d9dca1e316af2c55e38532b928757074233eb1b457fe02593525fa6f8a8841fd60d02eeb8fd55d69b53d8bec53666551e060ee3cd0367f905cfedab17a" },
                { "de", "db741554b833fcb951624fffba5aeff305100510f105952a827dc0da5a1abb8c717d3417856b6f5df73d968dca77b9fb24f2045ee12522fb94cf5814bb70c413" },
                { "dsb", "8328f1086edb3063e3ec5337e0ed21276faaada94781ba5b138c25f8906507468fc935cec42403b498c5407eb2853849f6757717509f52007f7f51cda47fdc5b" },
                { "el", "fe4ab5e37f9c1bbce7a4c04be880d34e1c849f81386bdc91e646b037c7b1d9fa738babc868ac6ecddc90a783b92a75274de54bb8c77af77939fc090da24c9074" },
                { "en-CA", "0aa00f1535edb0a709cfc79b9f674a5c3677bb2fe78ddbf57f78f06723168ecea54abaf459096bd83358f9f963db3cc191ceaabe36083a0639f257c114aa1e7c" },
                { "en-GB", "2cd4edd5067838b9bc7b70faa15f3c0f3083c4f8563e32d66e5ea4eabf22e7d7a8be332ebd85f0d7d138279e20bb2935add42e263f4982ef4f1d55d6f05c6f7e" },
                { "en-US", "3fa2926d6e74149841d2782890da0f4629a38117d5c242b5b6d69f0b46fc6a758482e4bae2141e8a695499f3fc53faf8d7d052c4a509659e6f55ddea0bbe01b9" },
                { "eo", "d26385e29a36095ec9f5a1bfd23c1165b929b7f842158b257a496c5170ea82d7985ed6564d0d675b1c51c36691c4133d79e297412d6e77a114f9cb8dda15fbba" },
                { "es-AR", "5c35ae0059cadb5e2c92ff0905510355a2a5aa4ea7b76fce9abea606f55754aa9a5013aba651c6dd3fcaece1811823218af03e85db985588da68fbf2f71fc343" },
                { "es-CL", "4488b895842895dc6a787c52418f3541e29abc41da591b99b3d629d3d905bd2c16b222cc8ea40b5d73120e93a20ab93eda5247c860a622f3ce518db1e880b456" },
                { "es-ES", "b37219bcff2abe28538450ac1a95eee5bad13e39336562e85ee5b0c25afa3e844da85d1ab597766e0502e8e2eb9509ddc636b96ebf88e6bea59eac8d19610302" },
                { "es-MX", "f1fc63eb3ee5a94202b8968402884795c38899c971d84a89924f8340b7953385f59e58c0e5aa958f25fe27c4aa8fb398e8aba743a390dcb62889acedfb79b8b0" },
                { "et", "50df187a1e5d02026452975ce3a9de03748c9e43c4fbef8e89ddf966d788c5d7ba1a545e947f7d163ec91e1ef24bf785c51e1b1abbfca7cea5496bcfbb28c92e" },
                { "eu", "4af7553d41dede9460614fbd6b2d9819cfd8104f249c9f28d724d5de9021de9a805d4f7af76d84be3d01101a60c17fd3ec345a852e43128b6c4c0525ad7e64fa" },
                { "fa", "1f05ab5ae63a3c18b4065a9a1a6a423d2033c256aa06c9f02fe4030eb04a276d2cabadf369bd9d25c9a828b2c3215508ccdb609e4678846317fa03ffbdd27912" },
                { "ff", "3169a9a4e9ce27b2ee96bf902c74b37bba4d70630002a1080061dc60733834eb18fe026274ea20eae69909fa0c1c47ef8ecc0e2f8632b170b7b80954ec4484d4" },
                { "fi", "ae9ead9cff4752b30ec650bfa6911111aecfd808c489003e297a6735e6e8551f5e25265aabfcb80ef1f43a73e8ecabae780500ff2ad8626d266f7aaae5718bee" },
                { "fr", "7fce558b9ec86db4032cd364feccbf8aa05f40e64f43d74d5d70797a25c723f479aceeccac988bc7a2d19b2732908a7eea2d441d0e7f25b6a94936996c09b645" },
                { "fur", "02ced76a9c68905824ae1ce62431a83aeab5273b0fa7d30230bc05f12df657bf9a5f49223e92efc0868648f41f4be66ceda35b108959b7d0d6e3acc234139f54" },
                { "fy-NL", "be6ae6aa5ef0e82631f9941ed360c597ade9576f4edd6fda84139a4dcdf2dd6d2bcd974bef6434c2f3e71b9eaaf531a022576178383e3cda39d272b3f846e08e" },
                { "ga-IE", "ed8f143a3062746dd3f3dc38150c9da39a748e7cb555a9f19c4ad88e3a16cd0251a8fe8a7576fb169212b5cea5a918239facc3fc6dbfd54060373758dd9e460c" },
                { "gd", "7e4bd6dea6a313b894f6f38be9e1f953146bcec8902d1fcffa1567fdcca7dcdadf08c82d82ef54d54d879c38e719e6950742eb0e14478afe92c8986290a24335" },
                { "gl", "b83a9ab523fbcec7cda2bd4df396c87e1f148726467a5ac69b5f6b2c1035f6c441b615b56479f1a3dd3c7c337adf6305f2be0e5cff346000e16389146e262df6" },
                { "gn", "98d0f48c0c58a0fa0022eb2ea2e3a91f4a34cfc22a10b37e2179d2e79c4adc8da31a41faccbaa60c423c6e56c169f0ab9db90f0fae6166c4375300775785e8f3" },
                { "gu-IN", "bc3bee6411440629b8ced630b54dccc66efce12150b1494b3c3bc4a81695d4493f7e11dd02bac93b8dda055e218efb23a18af4131fe7665231220d233663e842" },
                { "he", "aad4912b82efc7ba32e50c38a5896efd442032959465a79e4247f7818f2e4258894ac68b7d5505afb00278b18199840e2151bb4c09c8470d537d3976959e1eaf" },
                { "hi-IN", "af62c9d3229f81a2deb9a43c871f77634ae3dec3bcaec13764e825659c634d17897dcb5bc09fc58b8fcfd30ed9d3399a67e5380eac92beb5fd769504714b0581" },
                { "hr", "d1e51dd1c7c41687516f5dde78ef34d4aed11ae441188c1a81f82d713041ea5259d09c53179ee860723a0c59623fad5e92487eb4d97158386a4899eedb5a7cdc" },
                { "hsb", "01df69bb6df49a7f36d914ab35c035a09c6ff17c5da2039f822952c9dafc36bcec393db5a0ed5859b9d270caa7103cdb2474e3105fa67990f482dfddf2c17580" },
                { "hu", "2923f16de0dacb21680284019e2da80cc7956e7e354242777ecda720cefb4263a87e8be085ffbb2be16f04b259e9ce15c328f53341b27fd32894d661da42c46b" },
                { "hy-AM", "82b9786feaaeae2ba434c7033e0602332764d4f1b040f8845aebb77dafe58527f080ff21ffcd30427cfaee76e643315aedd94b0f115185bc8c61dcfd75dcc8d9" },
                { "ia", "17cb3bc8c195da093bcedc2db8fa520aa5cc28c4e879477f2dd7706e9e6b3ebadf3d1747228853cb6446106141f54a9f4303d1f76d84ed6853f9c968514e0b58" },
                { "id", "b6a1ef0e7b315d622c8978ade48a95f04f769efd9013e62c6e9e1db452904cda2889275e3edf7990848c177db218066dde934458d0aa47a08b722f82c4afcf94" },
                { "is", "3814a2259a71e7202df044d517c3d2ee31abd8e49f223190005a0cd4c36de9c4e2301e2a20498102def3b1af4e3b3a468674f36a0279b52473c77d542b70803e" },
                { "it", "a8c5ec53bdee08e19204e7e45d3f7ef82a58c2a346b03a82bd6e1af5b9759b6a13b11f967eb2ba1d0a7b03888bc7cb1d0eb4a88c56e5cebd868633b9bbc1355e" },
                { "ja", "c93cebb0d49e0d5c679bd806d71629fb73d894e167fa559412196c2dfc9ca59f2b95e24a876cc27554644fe6778b825b32677b21d1c28729b92473b7f29bb0ae" },
                { "ka", "30f92884c8496a6a5b24be148a103d6bd473b43862a13d10d0901297cd8bbe7aa988fa89edd43173711098e76b5886d1e0e886c7b7170f28d25cbe9abbf7053f" },
                { "kab", "e7abfd6c9c65a8cc4f3aa7db21550790d92c3c424a7004a43549cd14f832257e738d0f68741c09f3d68a3e1b10314b0a35a4c60e2bb35cd772dd90aab7253a71" },
                { "kk", "ebec574f62ad5b40ea0edd86156129a9c7f573d36694fc8bf7e6389d5199a346f5a577e4bc3dfe605258e19eef8a4c87170bdd8a63761cd456f373a339f562a4" },
                { "km", "8e770a6d09d63ae22e5c0a13431934542a105e109efa8b0603a0ee0ed9df6d1a6a109c6a8b934a0dda6b496504bfb346581111d94ae57dec7471d417e157abbb" },
                { "kn", "dea0df2a206d116a2387fb638d5a48b96eaf1651e3c2b3b2ee5115e6f129dc2588110be67c2c771c77866ed13ef8ca52e9f6b5a73f2706d39f52a8cff3e955ca" },
                { "ko", "8f976abd04e3c54fd295f1c91c8629f0b1c9e5d02e03a0ba56cc206d56cd32a11ccb9dc07d0c41aed5f3d6d6fb91a036e52b2473ec3f8714f2fcf4834a5c7e6d" },
                { "lij", "fb59b7a63369b1a7752a5ed72743f5919a82bb1bf1a0f01b392e2ec40aa236b98c4dedd2a6389f974aca3b5aa4d211daa83d286c6a5bd889d7f1e1bb5a7b0f97" },
                { "lt", "8d555eafcd8630a44a6c892ff57c197c6f7a5bec9db5e24e0c83234f8c48eedb60432ee5b7fe15fae57cf51181ad8a08512b97b663a2da76d445300493070fbe" },
                { "lv", "d6c3258a46c26ce99315fd2182f64090eb26d1a2966861e53515df3b8674c43ea907b77cabe62ba3147172c8905f94e16f3e32b3b408572a36f0639bd55e331d" },
                { "mk", "1c6afee21cc4e7d07c216f2acad672355893e8b4f9d160801ee92d58905ebbc60ddd30d7ae05bcccb09b37d7de011f33db15e470bebabfd526d433f4ee2a7b3c" },
                { "mr", "1f6f1ca69f9b6f4446e1c92fe4edded98cea33ed0a1512e14aac7f9dc56db020cccb19a73d751cd28af51d5469d214eeb2b22da9d7ddb6ce77187f083a97c664" },
                { "ms", "0f0c2e1cfb40bfb263e35068b85a77d549e700bd40155c662ab9959b3ca7280dea889ec069384548eae2e8d6d9b25b6999180989416aeb498d68e4b902e6791a" },
                { "my", "b217e363576a26d96c5933caa313702a79a15b6528cb2bf80da6333de8ab91448260c4e22e477cd3d346893a0a69abe3b5f90542f9d48be93227475cf04f43cf" },
                { "nb-NO", "7b906a2a7a807b4a453943ff8815bf5d2095b58e3b55de49d5925784c31dc3ab71df4996e9fa6f968c3e7393dbf001b9e3f0534e644bc3a283e00d75d8e8033b" },
                { "ne-NP", "06a9006acd4c882097415b72872b3e0f9af914832080f1e12a714aa8e4cd3d99bbf120060a8214a6c70996560d4d9791f6c8a9d085f68c22a437b234f41626ec" },
                { "nl", "b9391fe00ecba39b9817b8c61bb521551c109f3fefa04f63146bfa5c938349e0489b5f1ff22719dd1c1824a60d9aabc2dee8a6dad079aee458945783c19230f1" },
                { "nn-NO", "e55dd4acb4d070b1a51a519a318b5aecb83ea08ce107db7951b030709404462017930b856f56d317da84d8e95d08a83809a012c55693734880517ae4c19607df" },
                { "oc", "cf25d0185c16d07e4e9aa0b42355c0aaea13092046ae95ae1f033344e6f01fb0f87df71c32be25a5547844c1896792e90815019951670da8d3047c93e0256f7c" },
                { "pa-IN", "ba862e339900bc79bfd5a8b16f01d295ca9d995e9de1139bb257348913456a19a142425a573be7fe2871ee1f2d71ee490b5291ba5598ecba2bb9041ff40e4ffd" },
                { "pl", "49d1737af2f667bdd2df75d6b162c45b182dc9d02f1c5931730c1a27d4ce359307348bb15d9bc6454b04f687fa1c6b31c5979eb7d545244e80849419c461f0c3" },
                { "pt-BR", "17f5079f71706c7c198c9fce08293f84a09c59d08b0edb0044b3642cd8a44cc2e502fa93edf9e735bf2f042d9ab30462204cafe326c7b38276fd45ab80fd77e0" },
                { "pt-PT", "e589820845cd76de34302658d506deac0a54aed5fd3986fabed31b48e9d72481a31755786a62c1ed14f1dddf839f85d68136dfb27193a3d660c1b6918969bf8e" },
                { "rm", "a9a9253c2c8f7acccc41f339c008632b757f08a9588623e91e6a90ed281f23242de791ba7e992a3f7356cf78bc3b173a219c12db5809478d2ee39b2c98e80892" },
                { "ro", "8c797dec6f553f3bb28bb5c8a1e2d50f6b8572a05619c49ac37de685b52aed89a2b2229dd1b3429f2e153da96cdca417bc46abe235813c6aa86fb56495c957a5" },
                { "ru", "39b55481ace7d6fd91f62b07ac16de5e01523b9e4bc4a14e64f20936873b55e84b91132e1ef9eea32981c24dedd634f3635ed5ce80898b8b110f2a04fb838bd3" },
                { "sat", "55bc86c749bbaaa86b255382a4adf899620f41d2ea4c704e0c2fee91165c611cd01e6fa23ed4894b0e3a6f8f6c7e4398e89cd4fe9b0c6370ef7ac23d633eb58a" },
                { "sc", "0459c908dfe681de450a229981f897dfb564fa25db962f6f484936900b7866e0969431e79d517428b38bc1732ca680cce699c5d7548c71d6b88ee4deecec52d4" },
                { "sco", "ba840de5da047859ea529aae73de0988b786b83e84c5cf5662ce81c5c0b9042fa1cd4df43343443f4a9b3847450f191dae03d6745f21339e04eb8ddd64c591a6" },
                { "si", "4dbb2439f40e9dfebf2d1eb57af4dcde80143abcd17924e525f4b89bf13940f1b69a63b4b1758c6c421ea9be9f828628f4fdc8df7406511180bf4af65f487f31" },
                { "sk", "578eaf6a5a8073c518c9c206e897c3e2122728603837b76694c481c900a51ed176c4f7742edf81c05af2b33956d032eaec2b3f2df68fa528bd864f1d4526f09b" },
                { "skr", "333be32e2bb3054e4223e44c46b8222b6ea5a3ff782412aa4dbea1431ff46b22e82b6193404c277825170c74afcde8b723db51f7e202cd5c390108217007666c" },
                { "sl", "bfaac913926225619ef9ec3811d0bdecdfe27a1fd301ac8f62bcb91ff25f9a63a4748a258a95dc711ba2488149d1ea83b05a970c0bd82ecc98db1d7b2ae719a0" },
                { "son", "7cfd3d0ffe63d860ca27ceec9ffedb27b81cce0ef0311afe79d42468b843ea1032c74c18ae7252eb1f1b61e988c12191d41b067a75e623c608306862c3f60be8" },
                { "sq", "c5d857dba96f1ab9dc3db757f41d86fd2d60250ae08abd3172bff5e5885cbe1f801083bbbf9af586e32b23d78c647a0e2110f6f58d5b93655bd6a009839291e4" },
                { "sr", "013513e738906b585e10ec01a435c0136911b33858fab3b77974927b92ff24d74d0dee44e221d00c704fb57ac33dcecb5db5ceb9376cbd0b53462157198857f1" },
                { "sv-SE", "1270beaed1888c8d1700ddccacc17561121ed820ea97a038383d9c883154a99a77eb3d593b8ecc5825e436446a1c06968f1215b4ff6d1025ca08bc8ea801d84a" },
                { "szl", "82af475f56dcc4cc4f79ffa7185c3f8831e393d37edac1e34d2f742529ad28c5e8260e30df8e631c1ca57cec3e034c7ac0ffddbb1206cfdc3252db86ff85f6ee" },
                { "ta", "ac02eb6ff484b3f2c044edbed6621cefebc3375b996848df706c0913f94ccf906cb5774b7bd9873731b28d60ac187f75c22ddf8b68efb1f692cbe1fc5693e8b7" },
                { "te", "add2441be8b8374a64796000dc74538da06ca62bbaeddb7b0ffcb1511cd1ff0f541dcf765cf9f1f228a43231d5b878e7e6c35668f31cb0bf8d2b7befd3b4bc86" },
                { "tg", "ea38df4820e959928edc72e5b1f461505ad8e313a9ead7180f98aacbf690c1074c6f328dfb222a71968dd8982fa3d56d9d3c7e5820e3885b1ffa6dc5f3e6f677" },
                { "th", "da52601f54591dfd26ffa6c63d9261067410eedce8ff9369d90bb008a3207366b888127bbdb3c15573de045cc5650a5b74b1e424c9faff1005fdbc06cc352123" },
                { "tl", "6adc6dbd03aa8b22d4bcc23aacea79d8789a635221cbe954013b2cbf7327bb0a66aec7b9b94f21bf6c99983741eccb2b9895be70ecb1b16a48ea6d4113d4bcc4" },
                { "tr", "22aef2037c0f01bb1f5b9230ddde5f3398c6949acdcb7355981b37afd5dd857c96864bfa3fd52de0289287fb3e7b6aa12984fc3a02e72ad306329a04025e3fc2" },
                { "trs", "fbf91f2db103a9b1367954c015fd820f4c0551fa00d4d80234abe84a9036ca65643f718bc389bea0511f3e43699a7e18dd6b7a4e30a686206cc1b72f32a0d40c" },
                { "uk", "fb95538fb0477c1879da6167f54053d01505b336eff7e922b5b13a42246524e4555f4ecbb9bcf879c040f7c20853c8892fd616a84cf91732fa796f8b74dbb314" },
                { "ur", "c18d52cf9c1f430e11ed4ad531ce9d5813123b091033150021bc6c071b882bffb904695ec2c1dc34aeba0118f163c3f65ad1e602943c915f54539b2ae4718d9e" },
                { "uz", "d237ae06ff740b3f9c3fe9fba5dcd4b9009433adcb0c0a4c88afa9e20d77307e0b11b98c4db35aa53ff2abbb94c2b96a3bf56de13e1d408328528f5df4f1fe8d" },
                { "vi", "9b1e84923196708783a3d1372ca96c9e07ab74589e3f1e43692f7a41d4bd80685e3124469bf7e55b651d4c808fcfd1b5913d2ce03382c871b401602be91672e5" },
                { "xh", "da9519f78cf44dcfc2b8462a5e1377bc86715ace361921447f248cb1bb7664cf77bcd2434f888b70d0a959879f9c97af777e9d6e93d75663ab6171601f70b62b" },
                { "zh-CN", "cc95bf15af3a7df4f55893cb4c7eabd7462074945b9d78b0a7c804bf027057d7587116440fc250bbead440912a9cead1729a604e3d5d62cdd8739d502d12b4b8" },
                { "zh-TW", "cb906ecdba99d9f2c43299ee32dfaf4cb57ed7d256f0df57e83be0c569bc8efb4f9f40684a40d2e6b948b4a86b3d9377fd154187102d8c1add9221b24e217b59" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7a4bd9dae696b9974ce2290306df0c539dbefbaf2c6ddd1670c7585b2caa22f82f2c335d66fd8b28cb9d64166066e9bcd8dd04067aa421f5ffd6f7bc57003821" },
                { "af", "1f9270fcad036fedf157179583349d6fc2c81fd468c5c84366c912d0378d8c0126eb76dfc01e65a1125e87525f6f70b5fe38c40eb1cadced51805b1f07e03563" },
                { "an", "24afa732cc8910468e9f6d04ca21ed5178119db58bdf33ab5950668d353963a519b15741194f075bdb38a3701c327843fe89f6ef79130042f2d0b63e342db36b" },
                { "ar", "bcb29d6c3c9e66a7ea471798ca6f6be035c236f94c8c17e03c370b334f1a5e0eda338d1fce4afaeb67717f0ca43b1467b488dcf431f4756cf155a639ac0eaa9c" },
                { "ast", "c1050303ab6e87dee52cabb0b5d4e9e716508e602b29ddce64485f00980c1f91f254c5d6bbc91f4271a03cc3f78758306482dd4b4020ad4b941fd03746e636df" },
                { "az", "ad4b23acae8daba980537ae21e27c244b5e49b027e6affa6a6b81376830e562189056dc3435ce68617dd5d5e2451ffa42452e8d9691d7d2b75f7291f01c1b98f" },
                { "be", "22eed0a8c47156fb2332cd8d37d9d3f68089d9b79950e0575bd7dc68a78987fd4947814166597dec86e78de3bcd1dfc70ceaffbe6f64c0f59d33931c6c0b9e37" },
                { "bg", "9b345d773ebbf65e451e9c672c2170a58e31f2f96d5d762e2cb076064826f50b2c19c5553789a22f3120babde73453c6ef657bd5d987d696074906a1b9c86e83" },
                { "bn", "2e84f3454b8ded4b9ee455731b755d31f925b7b091cfba675bea85290013f32b792d0f4db9caf9a27590c62eeef05babb87ca86ea2f9d9cfeb7021ade49bd7e9" },
                { "br", "c3687d47c8bf7310dffb8f4f983ed37c82c757812870ff34115cb1d49252bb6788590b08050f1353c8a079bcbeca94e82cf5f1381a9caa60e5e85e15f23cbe9a" },
                { "bs", "85ae8705f85a147f3c2f4eb5ca486f1906b823bc3a48404529e32bc1fb9239a29e97cd7c80d0bbc8595144a2b93d3981e1af19920ff48cd9f717e586b16c4295" },
                { "ca", "e968a2b6de654e82234d8177dd2529e7dd498a864f7ccecceeccdd9bb16c0932bb23a6a3a51b961eab88592be04ed982d85f00884b6ce266bbaedeab3dfa672a" },
                { "cak", "72e9a90fb0ab9ba581fb942c8660539ee5c21e727bac04d3b1b9a9d45b1fae67c9e8708dbea0e78bbc50080ab15e18b6fed03887cbe709cbec7940b7baf6a4cc" },
                { "cs", "c2726e2df14a18957bf1f460ee5d7b30c3e27fb0d4b053d87f365cb04934660d19df54942876a682db2459bfb701dbc1667859ec16eee852bbb22db825c1a89a" },
                { "cy", "4ef02036c5bb6218a0e1ee497b9aacba77ec9873a034af5dac1e259d943bfac5619b309858b4221cc9df2198901e919d00f881047c5b5cf911d3ae381350c7d7" },
                { "da", "7fa1c9dfd97ca754cac63f09399bbb2465580ff683c4012e9b2ec597a35b68ab65e8280b0a77ce169c72fbba6d6fd0a6faaef95dfa2cff24d9221166155b161f" },
                { "de", "543ac853858259e11e392c680316beac1ee13bbc95e4a6914683b9201f83b895f377bc485b64d52a880313812507f60f1dff1358639aaa26cc7443180653c6c4" },
                { "dsb", "2e04626bdb1a70c1178ee2738f6eb9119114a8d051464dd2eec9eb4260c94e2b4bfa32a83e7f908141694188e3a05a64b190a38e72d0812a8d072f18258ef1f7" },
                { "el", "7bc8624d4765f930ff5da6e968519ed22287e3780e0b280ffb044602f3216dc6f8c60b731e83fea847ddda0cacb555b0065782e48bd5e558213050863cc75694" },
                { "en-CA", "76c9024c65c53fab09020b5e224a33f02434c347378e0433223b8b4a06adedf8d2e0a126de4f5b301f8a58ecdfa73983c73c575d6f1d6e6870a85c0bc96c2d95" },
                { "en-GB", "60b66d277ec5f0ea0cb9a45c0bc614f014eb5aa08bf5876bfa674804676dfe47ba596531c0847ee2674c9135df7b1a7c0aeee278f00c76875c671137fa2f537b" },
                { "en-US", "2c8ce784c31f8f8f2059af47db5bed2a39aac36b1b49e87bc3a7d343efa0b5b30e274bd12c25b84b48c2f1acc5421c79eeb60f7a612b152da253e3f9645dd61c" },
                { "eo", "03158d41f6877cb669077a0fef52b34aca265304d80c714454814e2d28a85cc4142a9f55ae0fc7f179528422b4dcb8e0d94301bccc634e74d80d883135250467" },
                { "es-AR", "bd3f85a25d3f5c10d8d9fdf3c69f9cfc1a492d85c8a6358596831612a4709d4d2cfd70f80f4fd240d1b57f1f5ee2d42e0ec3baa6eb09a05643d2395d852e11a9" },
                { "es-CL", "261140151ac7129efa945742f16a9877568887d006563b8ac2002ca7df8c73c488d6c44d44e4fa9d6573098a7df6ccc33cdc57163caf2d24dbaa50f5b66580d7" },
                { "es-ES", "e88cbef8f448f3922d5d818c002e048d95a4733790ff7c9f612e8aa8c85662d6b16f63075b790dd477a5b03194e8bdf4aaa0332bfd1dd5cb508800045ba04470" },
                { "es-MX", "59fe90fde92ab1f63af091094a5b2b644e699e6f8569c1f097d69c8e99c2b4eaca9653bbf8f68bb35c061283559ce302b58f852ae29308ff2a6b147d554b4caf" },
                { "et", "2d2987675f89313955eed1820de1a608aad196dc3df62b36d5a7beb911d249edaeb57b57831570fe5d09508c6dd2da6adf8a7a508090cf2ff801c122a24bd695" },
                { "eu", "f6b98af8605e7e4c54b0f250360f5e36226167eb0c70018d245c5d10e117152f2739d4d18b045a3a61b77d448fdca421184739c627dc82fd33c6c4480ca528e3" },
                { "fa", "00895196de863edf07726e6e8962ecf1fbd01e3f0abb94ce276d65d4a58f780516adc243d27e84061e4e8a44db61d3a9d1c7589a62f808750c2fa5230972c3e1" },
                { "ff", "99913921e57970ce02516e7df0de183c4d114c3e61759424180f26eac22e0ecb621332dfb88c2ed993d6db081da5531725bdf23e5634658f1f36a8ec2d5840ae" },
                { "fi", "efc5d1ecf70a11269d034e9709304c6ce81f6e0020cedd67d535ab9c10962945fff92cbebddb226073868745298e47401009ea81302f976548a4a42781e7a306" },
                { "fr", "fe338406dd996badfbcdcf654478d03dc427689c111d7ca2d85dca91b1a2f3106ce3e1e1e30cddff507f42abd066aae6ac544277b4113320b9d516f5de0b6ab1" },
                { "fur", "a42cba1cbcc840a8f668f6a4dd7cd9dd0319ddaf17109cdf70a7f40aeada8475b06b432b3edd05550646310326f9694ef05ee27e7ced25eb7a9a5002d36d3f3c" },
                { "fy-NL", "53f31fba516a422d16feae8b9664d57d382c70bdf6bb5ce6ac3cbc1f9c16eafdaf3f7c203e363c70c14248dfb2be29c7fe4947f7279b881ea39c3f0da5de5eef" },
                { "ga-IE", "6967b46f7a23b360670bc21a04584686dc0b3d6b87c2a7bbd4abeed8e16b5ba6eccca138f67a3059c7aa75b1217a961f94be917e6119aa7c682a307eeb160678" },
                { "gd", "40b0786d60c13854df471aa7c9d813e726fd4fb7893bed23d18ea9d33c16326e0646ca13b55a56c9ec42443b2dc1ab308b88321be8f45ff0606399abbfe1ea71" },
                { "gl", "b3af7ae50b26089a5189bc298d37ad879b8606bed28ba9430d751fc2df7f0110e752a691db0e10a23871df96173c14a65017767d1ac020ec2b71d8b4d276ab2e" },
                { "gn", "6cec8439e139e735768a8f719f0788f08f425d2e7f759ce56d997e6425e1d469580df9046fa674e16494c53150336d94a48caa6ee4927ee809eaa0d2b528d664" },
                { "gu-IN", "6bfa8b2d372537aadc24cbeb601f337fea3993b0310fea9f2b85877551b71c7900a96b73c6dc484d7f8f28c2a180d46d5d808dd9cf8b841cc05429db893d17a7" },
                { "he", "a42fd0992a1cb13148214ec220e9c76fc866b346ca93437c52a513888d5bdc195a4df57e683f2cda633ea72e2c1f521245064eece32b36bbafd4abb32f8b1243" },
                { "hi-IN", "a35792ae2b437941807eb21e625c57f018380cc8e2c1dd1b05ab4d6fa6b18a94d4d612f94cb1a7e1bd17c60a27ef8633a202ac8785090e08e15f3dd69e23dc65" },
                { "hr", "68cf72f039c744928ad91bb52643dacfd5c5997270ddd88beba6d7fecba40090f6e9cc505aaf5f8a82edc848b52dc061cb9fc978e617097fcd303de9c119fbc8" },
                { "hsb", "89d2cf2bd8b538c3544231e9374477a330a51440a4b58d9564f03782e069befe6d506ec21093a9e5486ef34d0eff93c55d27256e3099fe8a3520dbced0a24049" },
                { "hu", "0038224a3807c6dc8d33f6633f552d793f1188ae2d4ded1f4cb5d42f4bb11ab22e2a0b6d9fb6bb80de56eb55dff71258960ea4e12dce1bc0217201c06e9fdcfb" },
                { "hy-AM", "33e4e42e83bc35d59cc4bebd943c2e077d21305f9157bfae404ef6847afed1d356658128553105ec9c2178adf4e8669a6ac4e74185cc27717ca854cd84b3b78e" },
                { "ia", "3b4b23adb6a8e7462c84d2d467e20e695b96c4901a9216d9326df4a3d0f18e3b37773bfb751d1930bf9c53503397b7092f1d27880e0b3a8e80cd6c6ab7ff7852" },
                { "id", "7e3d69986e5e8f251e2590654ea2d866091c4cfb6300cab4d800ab834e4f02e69a014afdba449aa56e76feb0cd207591fc5fd7bffaedb66f7e7d9b210e589038" },
                { "is", "e5399fadc468ddf2e95623007a8c2354c666ee3b6e8b86c8d7d0d62d9d0398550860300a01261715a9ffc7b077f04439d237752f0f0b7967be899751e22cb828" },
                { "it", "77efd3711415fb566322cdc772a41155b76c2d9348a812f246ddfae37a34c5322f41d3f15c30f45d833fe166aa31c251fecbb4c141ce5fd2c828523e8ad60def" },
                { "ja", "e15009b1885fea63ed32dc8986ad17825b22bc7446e6f07c11d3ec528cbaca25d3e1cd4b4f91f031130ad957e0af2eec85436de5aedcbc38a4fd1d88d3fad62b" },
                { "ka", "f463fab27da55d444a29850f2e060fa79c1eea398060b0f745146b09aa90efde3f0bfa7982160f64c665811bbb7ef2faa290625a0ffd453bdef3635de14977b9" },
                { "kab", "2aba22dcffc48a82630aff35b0e819be8444aef2889e1abb4ff9733836c9540640820efcefee2eab1bcea7942295570af23fa210b21551877654275b673aad7c" },
                { "kk", "78aff38873ba0b6c6db145a0ebea4470f64743d0b44423993dad3d0b2a896864e44703fb3e9f491742e46e669e4d2684d5c848daa0c964b80b1ea7a7345f0099" },
                { "km", "de4ecec3a68cf7a253bc1755e9f01276655bc1a910d478b6afa02455e88fcd2aecb3d1be5c7a0710e1fd2dc8bebbfe1846cfa0ff1f9bec2350790d73a4dc96fd" },
                { "kn", "e9f75e49881148c2e6eb90b0723f84567d58f92c6e55976b6f7d04320a9bb77f57f8d910d0a618af35d635247fb59f156778742a1d11e6306c11933d482d525b" },
                { "ko", "0f9142fc957a0bec7f56c91c112e26697221819a6678ce62faea83bf574f0fd9ad6b9ba2972175790f7754b62baef3387de995b2da1ed8536ba61b37c48fb569" },
                { "lij", "287c9325a3a206c92d9c9a7629213b8384ec51dc185a11550ffc01cdf524f7bd7bf763fc2cd351ab944798ec4e76ba2ffeb6076a6f61b2f61745d4407d056e28" },
                { "lt", "281f6c7e297798b589a9aa4fd5ae92d2737d4ceb55376980e32fc1e5ca7450e51328c15b5329f9c5d4289d81b03b8652c11bf4fbb9507c4873452d707da76a3a" },
                { "lv", "bfb6531eead7e1d80b1b00924a3453358babba288d50844f9d6f566f941b8f16a8f93adc1ef11d7712ac14aa6943528e0e76ee59544dad12e76372672e754df8" },
                { "mk", "29b3d8910b7102f73e5752f1b2ba81775d3fc210332dd90fccda45fbd0cf5f6a74cefc34ee1c68fbd4dceb53500f8d4dbecb963f38737c3891b56222e539897d" },
                { "mr", "df3f7ba2b800ca7975bd23fd2d7184fb73af6fa2d747b8778d87d86be2a0b9e1a398bd6ef220aa0958709c6a8c7af6e90ced7b6a11d225f3933b0a9e459bb7e0" },
                { "ms", "af94d825b8cb30901cef40dfd87764c8c2edc2e936770c2c11aefa7677cd7aeb22a57e238ab997220dddbe4f4f8c14ed9727dbd228451ad2ce5f7d31e9888c00" },
                { "my", "eb32e6477dd05d0eaf5246734298a4c485f25cf378ea0fe1567691bb3df545568b0b0cbe35e6c19ed03c575560b4897d91061ab425ad5714b9c5a31a23e3c70d" },
                { "nb-NO", "9829402bca1133dbbf3d0237b91d6ec55f30f94b50d0ed20b31dda80add968ef792153aff386252e679a4884979a3301a2b5b11ca03d72111fbc71821702b30d" },
                { "ne-NP", "c28d43e1ccf25dbed8922a127181b3a55b3293378fca7c21b2ad6651d1f03cdc9e36d75104600b071e767eb66d465296eba0934035861df6dabbcb1fe6c2a908" },
                { "nl", "39ca76d196cc1a27e0c7de7915059b3ae4895cbb9c133cb7ac4e7f13a7aed141e4cb52450f5ce66fe34e72c41b8669aca3d79b820c227d7584248b75dff63343" },
                { "nn-NO", "25c61727937082754d9e91124ab45bc1cba82d7ec6b5eff06a2821d9487dcf5609607d30c82bbc92b029b2cd935075c5abd380f51e00cf8e4b06e3766d8173fe" },
                { "oc", "3fb31202e5d7040e067a1bffbe18f9bfd999865a1e5cf838ac7a35fda6a3d855e9f310757ce374cbd6cd99280549e8a267814cc3047bc30618ef4829d2b558af" },
                { "pa-IN", "346ffa7ae1fed4306582793b41c9bc1f5031984cdc43a23c5ab51d7f8f672584c554d6cd0b665f01399dfeddd75c04159d6725da0fa72da69617be7353d89120" },
                { "pl", "236d3f8df17285c7196c6b3ee79fa4093deddb0000336972e5101f95992815924452566f85878bf2461185d2af17fb8c47ed074b8e1d213e7b00cd88718c71de" },
                { "pt-BR", "9bc844482119b9c7820de496b30b72f26469ad3b02d007027fd09722e1449257f2332e2de04f3127c0ab0a585d12ecfac04b52704344c9dfc82dbe727ee7c2da" },
                { "pt-PT", "df284ad0c7d780581ca57e8ad92548d6b8c04b4f88b5c2df932c3d4612081045a9fa8868ca9808a0f5d0bca7290163e201a1da43ca15e699c2f394e452f6c165" },
                { "rm", "1d9b0e094fdec8824f6364125c5ade04660f778c5c9d13c172229b5ec2564decb00f27c3279c8a9d14aff94bffb6c797feb9e0d4f912efb0c66956f6ad4050c9" },
                { "ro", "4d67e9a9322640ee9c00fd6c4d947e300e0d5b54939a0cab109361b739c8d1153406028eedb4e2c48fb0c9107a8976dc7b00dd527f65cf3561a6811606ff92c1" },
                { "ru", "0a1eeaf75794de8944c262ffeaca1a83de04ed8ae7ae6a0aaafa361b0870d4d58d40927c3bbc5d3a1b0548fd89bc1b311f4356b54ca303a5ce231fc632066a58" },
                { "sat", "a86ac5095865cdd91025b4b0c9efde84ef80c19bb20de6a49eb7fa93929701aadb47be2b1ff31d59ef10eca25dad75a0604a5b8bb1d1516753c28c4aebc89dc7" },
                { "sc", "82092fd9fbe9aa3a90200c8adec4dfebd664252d5b774c63f7d61e4a24973a82de3e8abf1accfa6caf78ff81509c2adf9a25c9260779f49268405d83d85afe24" },
                { "sco", "0d4326b22152e3cd8ba4dd728fa3642f76fd0ed37db624a4630cafecc6e700db2f8b3b3cb09884698a78cabb01766c7fb14538c8ef12f4dd1aa936aabb841732" },
                { "si", "0a6175ab0753cb190f6e5c5285aebc333f2ec3a2e2dc6759af58e89765bd97b97af91c2e2c55cc7a9a97b4ae082fce6200ed19bc0455cc793776c01e042f8774" },
                { "sk", "c8b8d603a2004b48d8ba51d6b377e4c7e734e1d120cf4278b2a2a1786e460f946dd70a6b5929fd325a397050e5507bc7fe6294b18cbb7b7e50d6dc61eca38dcb" },
                { "skr", "7f362c8885c9e092210bf50cf38806426f358ae244aa9bc7b62cf1a2860ba9965c8ac635c52a721c5762723d4dd80a090812d834d419660154fb7824126829da" },
                { "sl", "270a7ecf151dd80c32a0c47365869b41d8184d0ea4eade949f3f1e373766918f0dda2bd6b62028acf52b00f828a627fcc7ba1909208a2e916fd8ae04fc39434f" },
                { "son", "0505fb66e779ffd2f2814c9eb01b69b332950eecac0a922897c6f1ba416b59711c006a6416437d0251ca53a3dc41ec9a50aabe6d61805cf278ab76a8de675c7c" },
                { "sq", "89a59ec5119cfd248bbceeed4b9e22dda2e70148caa9be86edc90efee86521cb1f672153b099a9d87afae778ece9ee28cb48907cdfe3ea0a4a4f2e8a9be09167" },
                { "sr", "0f520a8b6b14b4aa436ce6aa4adb1313d42f7fc9d3ce1d16adaea53398085753ef6ee8a4f451e301dd58ccd57220dc46a47f26d5ef8591a284c9297377788f1e" },
                { "sv-SE", "eab3434c81540576ed9692bbd43b5138680fa7dcb60358ad4f8cd2a01d957fd76f0de9413e99a31ea0ad5a0056ffe67a4fec6d35d52234c0c665fb878c2f56da" },
                { "szl", "e3c7c03de3718d03e063a3bc468aaa3a9eedb6fb173a1246328fd718cd97105cf8fe3a75113920f28a1998999e0958e73847fd3f11e158dffd2439a1e45b3f48" },
                { "ta", "c4678f79bf2efc6d4b0dd19d349ec49a670241d7aacd61ab548fac8bcc7d63a0f2cdf6d741473dfd7c75ab634383123d56b90f1998be18be9b52f575d7b486db" },
                { "te", "dfaf76877cf9791fcc5856b5c24a11126fc07a4eccfe4855fa3ec17b6c0975ec623a70175aeec64cbab6224c2f962ffa1a5e1134c967ad6a38a308d796809cd8" },
                { "tg", "d51db024bc9d7bbd8ed49ee2a63ab3658168548f3a110576023aa353e6ba4711a3482e5ada50d7cedcd254f1872e3e169b18468e3a23151739a93e4add5b461c" },
                { "th", "195f2ebe090a1ffbf4b61971b4691b352e5f045c83f6a3a6d12fc3b75aacef70cc36b82d851cb20ab411376c089e79bfd3297d2e39620cb8ce0a51a8781d8258" },
                { "tl", "4b9c14d81c699f2e0ce99e7c9e3435b7a8bb38f91e3b5b6b7c3b3368b2240a6dc817684f2cfcf83e4458cdea1ea276c14ed6e69dded150a4574b76f090e8131e" },
                { "tr", "9da25b5d30191a545a17dda5729eeb41474c35c31dfa349403a9798cecbeccfd5b7942cfefe7b3840c2028eb94ccd80331a2f169fcda6700270aec8b7e2906d4" },
                { "trs", "e18da744b83199ecc22c93c59e950c1b12321d0d767a2ae7ccc5dbe76870e533d20d84f10ba519d60ec15423e2a7e358b52a7dc4594902cf337d2f7d59521b30" },
                { "uk", "f5b82c8a6d2a340413bf6d0a4cf592700b92094ba06877e7a371dd6e5b20029835768f6db41948490197b5c1955c97b802c3afaebac2dc0629772975f02291d4" },
                { "ur", "aa2a03d5e4a7d6bb4d62ac4eced8cf0e05eea53de89517e4897f451653c06c3ed27336fd62dcaa0fdf7913c80f6666fc78bc1a7704c1ade097f2213d528d45e1" },
                { "uz", "eda7cd248ab3ef6329a817c25e7e8b5861470d4b6344346d22f08559734a4321652ac6aabea4b848d3cc7b272eb465f639d4e077965fe45deac1078ea8a5494e" },
                { "vi", "b9eae794ff18634c6d94e7aec6589d1f4e4ed52ccf5b84c62426d869bb214348e47d30aa8f4623bd8b5447fe0a8a554345dc550ea3e0bbffdccd473f1be6579d" },
                { "xh", "097ff306e4156dc2948ca3b9ca88181a51415f72fd903f1e6d87e54dd195e307afdd61298e904abe601a8bbc4dc7f98654a4c77ec1fa9cc50640cfa096b3c505" },
                { "zh-CN", "4ceb7adbfc11ba7a67a2bee01efd7f3b03f4842ec089637a1071ce390887ba68e8ae6e6e4186a282e4c6544dfbedba91bb8afd5a7b01550ea4b973b811a1a888" },
                { "zh-TW", "096df13c98acf8eb9f83366592b5a1ee1d643d07662b2cb352cef3ffd4e3183a27601d26c3f2628570bd28a629dbcc1623d329f553e2edca393071014bb8c3e2" }
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    cs32 = [];
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
                    cs64 = [];
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
            return [];
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
